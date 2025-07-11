using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PropertyManagementAPI.Application.Configuration;
using PropertyManagementAPI.Application.Services.Accounting.Quickbooks;
using PropertyManagementAPI.Application.Services.Tenants;
using PropertyManagementAPI.Domain.DTOs.Quickbooks;
using PropertyManagementAPI.Infrastructure.Quickbooks;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/accounting/quickbooks")]
    [Tags("Quickbooks-Callback")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Produces("application/json")]
    public class QuickBooksCallbackController : ControllerBase
    {
        private readonly IQuickBooksTokenManager _tokenManager;
        private readonly ITenantService _tenantService;
        private readonly QuickBooksOptions _qbOptions;
        private readonly ILogger<QuickBooksCallbackController> _logger;

        public QuickBooksCallbackController(
            IQuickBooksTokenManager tokenManager,
            ITenantService tenantService,
            IOptions<QuickBooksOptions> qbOptions,
            ILogger<QuickBooksCallbackController> logger)
        {
            _tokenManager = tokenManager;
            _tenantService = tenantService;
            _qbOptions = qbOptions.Value;
            _logger = logger;
        }

        /// <summary>
        /// Handles the QuickBooks OAuth callback, exchanges code for token, and stores RealmId.
        /// </summary>
        [HttpGet("callback")]
        public async Task<IActionResult> Callback(
            [FromQuery] string code,
            [FromQuery] string realmId,
            [FromQuery] string state)
        {
            _logger.LogInformation("QuickBooks OAuth Callback: code={Code}, realmId={RealmId}, state={State}", code, realmId, state);

            try
            {
                var tokenSet = await _tokenManager.ExchangeAuthCodeForTokenAsync(realmId, code);
                if (tokenSet == null)
                {
                    _logger.LogError("Token exchange failed — no token set returned.");
                    return StatusCode(500, "Failed to exchange QuickBooks authorization code.");
                }

                if (!int.TryParse(state, out var tenantId))
                {
                    _logger.LogWarning("Invalid state parameter: {State}", state);
                    return BadRequest("Invalid or missing tenant reference.");
                }

                var success = await _tenantService.LinkQuickBooksAccountAsync(
                    tenantId,
                    tokenSet.AccessToken,
                    tokenSet.RefreshToken,
                    realmId
                );

                if (!success)
                {
                    _logger.LogError("Failed to persist QuickBooks credentials for tenant {TenantId}", tenantId);
                    return StatusCode(500, "Failed to link QuickBooks account.");
                }

                _logger.LogInformation("Tenant {TenantId} successfully linked to realm {RealmId}", tenantId, realmId);
                return Ok("QuickBooks account linked successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OAuth callback processing failed.");
                return StatusCode(500, "Error linking QuickBooks account.");
            }
        }
    }
}