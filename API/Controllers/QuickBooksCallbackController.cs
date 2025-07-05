using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Application.Services.Auth;
using PropertyManagementAPI.Application.Services.Tenants;
using PropertyManagementAPI.Application.Services.Quickbooks;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/auth/quickbooks")]
    public class QuickBooksCallbackController : ControllerBase
    {
        private readonly ITenantService _tenantService;
        private readonly IQuickBooksAuthService _quickBooksAuthService;
        private readonly IStateManager _stateManager;
        private readonly ILogger<QuickBooksCallbackController> _logger;

        public QuickBooksCallbackController(ITenantService tenantService,IQuickBooksAuthService quickBooksAuthService,IStateManager stateManager,ILogger<QuickBooksCallbackController> logger)
        {
            _tenantService = tenantService;
            _quickBooksAuthService = quickBooksAuthService;
            _stateManager = stateManager;
            _logger = logger;
        }

        [HttpGet("callback")]
        public async Task<IActionResult> QuickBooksCallback(
            [FromQuery] string code,
            [FromQuery] string realmId,
            [FromQuery] string state)
        {
            if (string.IsNullOrWhiteSpace(code) ||
                string.IsNullOrWhiteSpace(realmId) ||
                string.IsNullOrWhiteSpace(state))
            {
                return BadRequest("Missing required QuickBooks parameters.");
            }

            try
            {
                var tokenResponse = await _quickBooksAuthService.ExchangeAuthCodeForTokenAsync(code);
                var tenantId = _stateManager.ResolveTenantIdFromState(state);

                await _tenantService.LinkQuickBooksAccountAsync(
                    tenantId,
                    tokenResponse.AccessToken,
                    tokenResponse.RefreshToken,
                    realmId);

                _logger.LogInformation("Tenant {TenantId} successfully linked to QuickBooks realm {RealmId}.",
                    tenantId, realmId);

                return Redirect("/quickbooks/connected");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "QuickBooks OAuth callback failed.");
                return StatusCode(500, "An error occurred while linking QuickBooks.");
            }
        }
    }
}