using Going.Plaid;
using Going.Plaid.Entity;
using Going.Plaid.Link;

namespace PropertyManagementAPI.Application.Services.Payments.Plaid
{
    public class PlaidLinkService : IPlaidLinkService
    {
        private readonly PlaidClient _plaidClient;

        public PlaidLinkService(PlaidClient plaidClient)
        {
            _plaidClient = plaidClient;
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

    }
}
