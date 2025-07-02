#if DEBUG
using Going.Plaid;
using Going.Plaid.Entity;
using Going.Plaid.Link;
using Going.Plaid.Sandbox;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Application.Services.Payments.Plaid;

namespace PropertyManagementAPI.API.Controllers.Test
{
    [ApiController]
    [Route("api/test/plaid")]
    //[ApiExplorerSettings(GroupName = "PlaidTest")]
    public class PlaidTestController : ControllerBase
    {
        private readonly IPlaidService _plaidService;
        private readonly IPlaidSandboxTestService _sandboxTestService;
        private readonly IPlaidLinkService _plaidLinkService;
        private readonly ILogger<PlaidTestController> _logger;
        private const string SandboxInstitutionId = "ins_109508";

        public PlaidTestController(IPlaidService plaidService, IPlaidSandboxTestService sandboxTestService, IPlaidLinkService plaidLinkService, ILogger<PlaidTestController> logger)
        {
            _plaidService = plaidService;
            _sandboxTestService = sandboxTestService;
            _plaidLinkService = plaidLinkService;
            _logger = logger;
        }
        [HttpPost("create-link-token")]
        public async Task<IActionResult> CreateLSandboxLinkToken()
        {
            var token = await _plaidLinkService.CreateLinkTokenAsync();
            return Ok(new { link_token = token });
        }

        [HttpPost("create-sandbox-public-token")]
        public async Task<IActionResult> CreateSandboxPublicToken()
        {
            var token = await _sandboxTestService.CreateSandboxPublicTokenAsync();
            return Ok(new { public_token = token });
        }

        [HttpPost("create-sandbox-full-token")]
        public async Task<IActionResult> CreateFullSandboxPublicToken()
        {
            var token = await _sandboxTestService.CreateFullSandboxPublicTokenAsync();
            return Ok(new { public_token = token });
        }

        public class ExchangeTokenRequest
        {
            public string PublicToken { get; set; }
        }

        [HttpPost("exchange-public-token")]
        public async Task<IActionResult> ExchangeSandboxPublicToken([FromBody] ExchangeTokenRequest request)
        {
            var accessToken = await _plaidService.ExchangePublicTokenAsync(request.PublicToken);
            return Ok(new { access_token = accessToken });
        }

        [HttpGet("accounts")]
        public async Task<IActionResult> GetSandboxAccounts([FromQuery] string accessToken)
        {
            var accounts = await _plaidService.GetAccountsAsync(accessToken);
            return Ok(accounts);
        }
    }
}
#endif