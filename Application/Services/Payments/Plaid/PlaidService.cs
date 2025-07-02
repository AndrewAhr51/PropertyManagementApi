using Going.Plaid;
using Going.Plaid.Auth;
using Going.Plaid.Entity;
using Going.Plaid.Item;
using Going.Plaid.Link;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PropertyManagementAPI.Application.Services.Payments.Plaid
{
    public class PlaidService : IPlaidService
    {
        private readonly PlaidClient _plaidClient;
        private readonly ILogger<PlaidService> _logger;

        public PlaidService(PlaidClient plaidClient, ILogger<PlaidService> logger)
        {
            _plaidClient = plaidClient;
            _logger = logger;
        }

        public async Task<string> CreateLinkTokenAsync()
        {
            var request = new LinkTokenCreateRequest
            {
                ClientName = "Demo Property App",
                Language = Language.English,
                CountryCodes = new[] { CountryCode.Us },
                Products = new[] { Products.Auth },
                User = new LinkTokenCreateRequestUser
                {
                    ClientUserId = Guid.NewGuid().ToString()
                }
            };

            var response = await _plaidClient.LinkTokenCreateAsync(request);

            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(response.Error?.DisplayMessage ?? "Failed to create link token.");

            return response.LinkToken;
        }


        public async Task<string> ExchangePublicTokenAsync(string publicToken)
        {
            var response = await _plaidClient.ItemPublicTokenExchangeAsync(new ItemPublicTokenExchangeRequest
            {
                PublicToken = publicToken
            });

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to exchange public token: {Error}", response.Error?.DisplayMessage);
                throw new Exception("Plaid token exchange failed.");
            }

            return response.AccessToken;
        }

        public async Task<IEnumerable<Going.Plaid.Entity.Account>> GetAccountsAsync(string accessToken)
        {
            var response = await _plaidClient.AuthGetAsync(new AuthGetRequest
            {
                AccessToken = accessToken
            });

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch bank info: {Error}", response.Error?.DisplayMessage);
                throw new Exception("Failed to get bank account details.");
            }

            return response.Accounts;
        }
    }
}
