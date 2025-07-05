using Going.Plaid;
using Going.Plaid.Entity;
using Going.Plaid.Link;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Domain.DTOs.Banking;
using PropertyManagementAPI.Application.Services.Tenants;
using PropertyManagementAPI.Application.Services.Payments.Plaid;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/plaid")]
    public class PlaidController : ControllerBase
    {
        private readonly IPlaidService _plaidService;
        private readonly IPlaidLinkService _plaidLinkService;
        private readonly ITenantOnboardingService _onboardingService;

        public PlaidController(
            IPlaidService plaidService,
            IPlaidLinkService plaidLinkService,
            ITenantOnboardingService onboardingService)
        {
            _plaidService = plaidService;
            _plaidLinkService = plaidLinkService;
            _onboardingService = onboardingService;
        }

        [HttpPost("exchange-token")]
        public async Task<IActionResult> ExchangeToken([FromBody] string publicToken)
        {
            var accessToken = await _plaidService.ExchangePublicTokenAsync(publicToken);
            return Ok(new { accessToken });
        }

        [HttpGet("accounts")]
        public async Task<IActionResult> GetAccounts([FromQuery] string accessToken)
        {
            var accounts = await _plaidService.GetAccountsAsync(accessToken);
            return Ok(accounts);
        }

        [HttpPost("create-link-token")]
        public async Task<IActionResult> CreateLinkToken()
        {
            var token = await _plaidLinkService.CreateLinkTokenAsync();
            return Ok(new { link_token = token });
        }

        [HttpPost("exchange-public-token")]
        public async Task<IActionResult> ExchangePublicToken([FromBody] ExchangeTokenRequest request)
        {
            var accessToken = await _plaidService.ExchangePublicTokenAsync(request.PublicToken);
            await _onboardingService.OnPlaidAccountVerified(request.PropertyId, request.TenantId);
            return Ok(new { access_token = accessToken });
        }
    }
}