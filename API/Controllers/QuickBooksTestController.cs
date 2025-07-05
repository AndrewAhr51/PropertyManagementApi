#if DEBUG
using Intuit.Ipp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PropertyManagementAPI.Application.Configuration;
using PropertyManagementAPI.Application.Services.Accounting.Quickbooks;
using PropertyManagementAPI.Application.Services.Invoices;
using PropertyManagementAPI.Application.Services.Tenants;
using PropertyManagementAPI.Domain.DTOs.Invoices;
using PropertyManagementAPI.Domain.DTOs.Quickbooks;
using Stripe;
using Customer = Intuit.Ipp.Data.Customer;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/test/quickbooks")]
    public class QuickBooksTestController : ControllerBase
    {
        private readonly QuickBooksAuthSettings _authSettings;
        private readonly QuickBooksTokenManager _tokenManager;
        private readonly QuickBooksInvoiceService _qbInvoiceService;
        private readonly ITenantService _tenantService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<QuickBooksTestController> _logger;

        public QuickBooksTestController(
            QuickBooksTokenManager tokenManager,
            QuickBooksInvoiceService qbInvoiceService,
            ITenantService tenantService,
            IInvoiceService invoiceService,
            ILogger<QuickBooksTestController> logger,
            QuickBooksAuthSettings authSettings)
        {
            _tokenManager = tokenManager;
            _qbInvoiceService = qbInvoiceService;
            _tenantService = tenantService;
            _invoiceService = invoiceService;
            _logger = logger;
            _authSettings = authSettings;
        }

        /// <summary>
        /// Displays the current QuickBooks configuration values for debugging.
        /// </summary>
        /// <returns>QuickBooksOptions values (clientId, secret, redirectUri).</returns>
        [HttpGet("debug-config")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult DebugQuickBooksConfig()
        {
            return Ok(new
            {
                ClientId = _authSettings.ClientId,
                Secret = string.IsNullOrEmpty(_authSettings.ClientSecret) ? "(empty)" : "***",
                RedirectUri = _authSettings.RedirectUri
            });
        }

        /// <summary>
        /// Initiates QuickBooks OAuth flow and redirects to Intuit for tenant authorization.
        /// </summary>
        /// <param name="tenantId">The ID of the tenant to authorize with QuickBooks.</param>
        /// <returns>Redirects to Intuit OAuth authorization screen.</returns>
        [HttpGet("authorize")]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public IActionResult AuthorizeTenant([FromQuery] int tenantId)
        {
            if (string.IsNullOrWhiteSpace(_authSettings.RedirectUri))
            {
                _logger.LogError("Missing RedirectUri in QuickBooks settings.");
                return StatusCode(500, "Missing RedirectUri configuration.");
            }

            var redirectUri = Uri.EscapeDataString(_authSettings.RedirectUri);
            var scopes = Uri.EscapeDataString("com.intuit.quickbooks.accounting");
            var state = tenantId.ToString();

            var authUrl = $"https://appcenter.intuit.com/connect/oauth2?" +
                          $"client_id={_authSettings.ClientId}&" +
                          $"redirect_uri={redirectUri}&" +
                          $"response_type=code&" +
                          $"scope={scopes}&" +
                          $"state={state}";

            return Redirect(authUrl);
        }


        [HttpGet("generate-token")]
        public async Task<IActionResult> GenerateToken()
        {
            try
            {
                var token = await _tokenManager.GetTokenAsync(); // Optional sandbox stub
                return Ok(new { access_token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "QuickBooksTest: Failed to generate token.");
                return StatusCode(500, "Token generation failed.");
            }
        }


        /// <summary>
        /// Receives OAuth callback from QuickBooks, exchanges code for tokens, and links tenant.
        /// </summary>
        /// <param name="code">Authorization code from QuickBooks.</param>
        /// <param name="realmId">QuickBooks company identifier.</param>
        /// <param name="state">Tenant ID passed through OAuth flow.</param>
        /// <returns>Linking status message.</returns>
        [HttpGet("callback")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> QuickBooksCallback(
            [FromQuery] string code,
            [FromQuery] string realmId,
            [FromQuery] string state)

        {
            _logger.LogInformation("QuickBooksCallback: code={Code}, realmId={RealmId}, state={State}", code, realmId, state);

            try
            {
                var tokenSet = await _tokenManager.ExchangeAuthCodeForTokenAsync(code);
                if (tokenSet == null) return StatusCode(500, "Token exchange failed.");

                if (!int.TryParse(state, out var tenantId))
                {
                    _logger.LogWarning("QuickBooksCallback: Invalid state value {State}", state);
                    return BadRequest("Invalid tenant identifier.");
                }

                var saved = await _tenantService.LinkQuickBooksAccountAsync(
                    tenantId,
                    tokenSet.AccessToken,
                    tokenSet.RefreshToken,
                    realmId
                );

                if (!saved)
                {
                    _logger.LogError("QuickBooksCallback: Failed to link QuickBooks account for tenant {TenantId}", tenantId);
                    return StatusCode(500, "Linking failed.");
                }

                _logger.LogInformation("QuickBooksCallback: Tenant {TenantId} linked to realm {RealmId}", tenantId, realmId);
                return Ok("QuickBooks account successfully linked.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "QuickBooksCallback: Error during callback.");
                return StatusCode(500, "Callback processing error.");
            }
        }

        /// <summary>
        /// Syncs a local invoice to QuickBooks for a given tenant.
        /// </summary>
        /// <param name="tenantId">Tenant ID whose QuickBooks account is already linked.</param>
        /// <param name="invoiceId">Invoice ID to sync.</param>
        /// <returns>QuickBooks invoice object.</returns>
        [HttpPost("sync-invoice-quickbooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SyncInvoiceToQuickBooksAsync(
            [FromQuery] int tenantId,
            [FromBody] int invoiceId)
        {
            _logger.LogInformation("SyncInvoice: tenant={TenantId}, invoice={InvoiceId}", tenantId, invoiceId);

            try
            {
                var tenant = await _tenantService.GetTenantByIdAsync(tenantId);
                if (tenant == null || string.IsNullOrWhiteSpace(tenant.RealmId))
                    return BadRequest("Tenant or realmId not found.");

                var localInvoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
                if (localInvoice == null)
                    return NotFound("Invoice not found.");

                var customer = new Customer
                {
                    Id = localInvoice.TenantId.ToString(),
                    DisplayName = localInvoice.TenantName,
                    PrimaryEmailAddr = new EmailAddress { Address = localInvoice.Email }
                };

                var lines = localInvoice.LineItems.Select(item =>
                {
                    var detail = new SalesItemLineDetail
                    {
                        ItemRef = new ReferenceType
                        {
                            name = item.LineItemTypeName ?? "Service",
                            Value = "123" // Replace with actual item ID
                        }
                    };

                    return new Line
                    {
                        DetailType = LineDetailTypeEnum.SalesItemLineDetail,
                        Amount = item.Amount,
                        Description = item.Description ?? "Invoice line item",
                        AnyIntuitObject = detail
                    };
                }).ToList();

                var itemId = lines.FirstOrDefault()?.AnyIntuitObject is SalesItemLineDetail detail
                    ? detail.ItemRef.Value
                    : "123";

                var qbInvoice = await _qbInvoiceService.CreateInvoiceAsync(
                    tenant.RealmId,
                    customer,
                    localInvoice.LineItems.Sum(li => li.Amount),
                    itemId
                );

                return Ok(qbInvoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncInvoice: Error syncing invoice for tenant {TenantId}", tenantId);
                return StatusCode(500, "Invoice sync failed.");
            }
        }
    }
}
#endif