#if DEBUG

using CorrelationId;
using CorrelationId.Abstractions;
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
using PropertyManagementAPI.Domain.Responses;
using PropertyManagementAPI.Application.Services.Intuit;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/test/quickbooks")]
    public class QuickBooksTestController : ControllerBase
    {
        private readonly ILogger<QuickBooksTestController> _logger;
        private readonly IQuickBooksTokenManager _tokenManager;
        private readonly IQuickBooksInvoiceService _invoiceService;
        private readonly ITenantService _tenantService;
        private readonly IInvoiceService _localInvoiceService;
        private readonly IOptions<QuickBooksAuthSettings> _authOptions;
        private readonly IItemReferenceResolver _itemResolver;
        private readonly ICorrelationContextAccessor _correlation;

        public QuickBooksTestController(
            IQuickBooksTokenManager tokenManager,
            IQuickBooksInvoiceService invoiceService,
            ITenantService tenantService,
            IInvoiceService localInvoiceService,
            ILogger<QuickBooksTestController> logger,
            IOptions<QuickBooksAuthSettings> authOptions,
            IItemReferenceResolver itemResolver,
            ICorrelationContextAccessor correlation)
        {
            _tokenManager = tokenManager;
            _invoiceService = invoiceService;
            _tenantService = tenantService;
            _localInvoiceService = localInvoiceService;
            _logger = logger;
            _authOptions = authOptions;
            _itemResolver = itemResolver;
            _correlation = correlation;
        }

        [HttpGet("debug-config")]
        public IActionResult DebugQuickBooksConfig()
        {
            var config = _authOptions.Value;
            return Ok(new DebugApiResponse("QuickBooks config loaded", new
            {
                ClientId = config.ClientId,
                Secret = "(hidden)",
                RedirectUri = config.RedirectUri
            }));
        }

        [HttpPost("authorize-url")]
        public IActionResult GetAuthorizationUrl([FromQuery] int tenantId)
        {
            var uri = GenerateQuickBooksAuthUrl(tenantId);
            return Ok(new { redirectUrl = uri });
        }

        [HttpGet("authorize")]
        public IActionResult AuthorizeTenant([FromQuery] int tenantId)
        {
            var redirectUri = Uri.EscapeDataString(_authOptions.Value.RedirectUri);
            var scopes = Uri.EscapeDataString("com.intuit.quickbooks.accounting");
            var authUrl = $"https://appcenter.intuit.com/connect/oauth2?" +
                          $"client_id={_authOptions.Value.ClientId}&" +
                          $"redirect_uri={redirectUri}&" +
                          $"response_type=code&" +
                          $"scope={scopes}&" +
                          $"state={tenantId}";
            return Redirect(authUrl);
        }

        [HttpGet("generate-token")]
        public async Task<IActionResult> GenerateToken(string realmid)
        {
            try
            {
                var token = await _tokenManager.GetTokenAsync(realmid);
                return Ok(new DebugApiResponse("Token generated", new { token }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate QuickBooks token.");
                return StatusCode(500, new DebugApiResponse("Token generation failed"));
            }
        }

        [HttpGet("ping")]
        public async Task<IActionResult> VerifyQuickBooksConnection([FromQuery] int tenantId)
        {
            var tenant = await _tenantService.GetTenantByIdAsync(tenantId);
            if (tenant == null || string.IsNullOrWhiteSpace(tenant.RealmId))
                return BadRequest(new DebugApiResponse("Tenant or realmId not found"));

            var result = await _invoiceService.VerifyConnectionAsync(tenant.RealmId);
            return Ok(new DebugApiResponse("Ping result", new { connected = result }));
        }

        [HttpGet("callback")]
        public async Task<IActionResult> QuickBooksCallback(
        [FromQuery] string code,
        [FromQuery] string realmId,
        [FromQuery] string state)
        {
            var traceId = _correlation.CorrelationContext?.CorrelationId;

            _logger.LogInformation("OAuth callback received: code={Code}, realmId={RealmId}, state={State}, trace={TraceId}",
                code, realmId, state, traceId);

            if (!int.TryParse(state, out var tenantId))
                return BadRequest(new DebugApiResponse("Invalid tenant identifier"));

            try
            {
                var tokenSet = await _tokenManager.ExchangeAuthCodeForTokenAsync(realmId, code);
                if (tokenSet == null || string.IsNullOrWhiteSpace(tokenSet.AccessToken))
                {
                    _logger.LogError("Token exchange failed for tenantId={TenantId}, trace={TraceId}", tenantId, traceId);
                    return StatusCode(500, new DebugApiResponse("Token exchange failed"));
                }

                var saved = await _tenantService.LinkQuickBooksAccountAsync(
                    tenantId,
                    tokenSet.AccessToken,
                    tokenSet.RefreshToken,
                    realmId
                );

                if (!saved)
                {
                    _logger.LogError("Failed to persist QuickBooks token for tenantId={TenantId}, trace={TraceId}", tenantId, traceId);
                    return StatusCode(500, new DebugApiResponse($"Failed to link QuickBooks account for tenant {tenantId}"));
                }

                _logger.LogInformation("Tenant {TenantId} successfully linked to QuickBooks realm {RealmId}, trace={TraceId}",
                    tenantId, realmId, traceId);

                return Ok(new DebugApiResponse("QuickBooks account linked successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OAuth callback error: tenantId={TenantId}, trace={TraceId}", tenantId, traceId);
                return StatusCode(500, new DebugApiResponse("OAuth callback failed"));
            }
        }

        [HttpPost("sync-invoice-quickbooks")]
        public async Task<IActionResult> SyncInvoiceToQuickBooks([FromQuery] int tenantId, [FromBody] int invoiceId)
        {
            _logger.LogInformation("SyncInvoice invoked: tenantId={TenantId}, invoiceId={InvoiceId}, trace={TraceId}",
                tenantId, invoiceId, _correlation.CorrelationContext?.CorrelationId);

            try
            {
                var tenant = await _tenantService.GetTenantByIdAsync(tenantId);
                if (tenant == null || string.IsNullOrWhiteSpace(tenant.RealmId))
                    return BadRequest(new DebugApiResponse("Tenant or realmId not found"));

                var localInvoice = await _localInvoiceService.GetInvoiceByIdAsync(invoiceId);
                if (localInvoice == null)
                    return NotFound(new DebugApiResponse("Invoice not found"));

                var customer = new Customer
                {
                    Id = localInvoice.TenantId.ToString(),
                    DisplayName = localInvoice.TenantName,
                    PrimaryEmailAddr = new EmailAddress { Address = localInvoice.Email }
                };

                var lines = localInvoice.LineItems.Select(item =>
                {
                    var resolvedItemId = _itemResolver.ResolveItemId(item.LineItemTypeName ?? "Service") ?? "default";

                    var detail = new SalesItemLineDetail
                    {
                        ItemRef = new ReferenceType
                        {
                            name = item.LineItemTypeName ?? "Service",
                            Value = resolvedItemId
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

                var itemId = lines.FirstOrDefault()?.AnyIntuitObject is SalesItemLineDetail d ? d.ItemRef.Value : "default";

                var qbInvoice = await _invoiceService.CreateInvoiceAsync(
                    tenant.RealmId,
                    customer,
                    localInvoice.LineItems.Sum(li => li.Amount),
                    itemId
                );

                var dto = new QuickBooksInvoiceDto
                {
                    Id = qbInvoice.Id,
                    TotalAmount = qbInvoice.TotalAmt,
                    SyncStatus = "Success"
                };

                return Ok(new DebugApiResponse("Invoice synced", dto));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SyncInvoice error: tenantId={TenantId}, trace={TraceId}", tenantId, _correlation.CorrelationContext?.CorrelationId);
                return StatusCode(500, new DebugApiResponse("Invoice sync failed"));
            }
        }
        private string GenerateQuickBooksAuthUrl(int tenantId)
        {
            var clientId = _authOptions.Value.ClientId;
            var redirectUri = Uri.EscapeDataString(_authOptions.Value.RedirectUri ?? "");
            var scopes = Uri.EscapeDataString("com.intuit.quickbooks.accounting");

            return $"https://appcenter.intuit.com/connect/oauth2?" +
                   $"client_id={clientId}&" +
                   $"redirect_uri={redirectUri}&" +
                   $"response_type=code&" +
                   $"scope={scopes}&" +
                   $"state={tenantId}";
        }
    }
}
#endif