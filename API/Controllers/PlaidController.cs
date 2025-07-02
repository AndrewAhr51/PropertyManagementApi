using Going.Plaid;
using Going.Plaid.Entity;
using Going.Plaid.Link;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Payments.Plaid;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/plaid")]
    public class PlaidController : ControllerBase
    {
        private readonly IPlaidService _plaidService;
        private readonly IPlaidLinkService _plaidLinkService;

        public PlaidController(IPlaidService plaidService, IPlaidLinkService plaidLinkService)
        {
            _plaidService = plaidService;
            _plaidLinkService = plaidLinkService;
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
            var accounts = await _plaidService.GetBankAccountsAsync(accessToken);
            return Ok(accounts);
        }

        [HttpPost("create-link-token")]
        public async Task<IActionResult> CreateLinkToken()
        {
            var token = await _plaidLinkService.CreateLinkTokenAsync();
            return Ok(new { link_token = token });
        }

    }
}
