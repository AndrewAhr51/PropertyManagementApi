using Going.Plaid;
using Going.Plaid.Sandbox;
using Going.Plaid.Entity;

namespace PropertyManagementAPI.Application.Services.Payments.Plaid
{
    public class PlaidSandboxTestService : IPlaidSandboxTestService
    {
        private readonly PlaidClient _plaidClient;

        public PlaidSandboxTestService(PlaidClient plaidClient)
        {
            _plaidClient = plaidClient;
        }

        public async Task<string> CreateSandboxPublicTokenAsync()
        {
            var response = await _plaidClient.SandboxPublicTokenCreateAsync(new SandboxPublicTokenCreateRequest
            {
                InstitutionId = "ins_109508",
                InitialProducts = new[] { Products.Auth }
            });

            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(response.Error?.DisplayMessage ?? "Failed to create sandbox public token.");

            return response.PublicToken;
        }

        public async Task<string> CreateFullSandboxPublicTokenAsync()
        {
            var response = await _plaidClient.SandboxPublicTokenCreateAsync(new SandboxPublicTokenCreateRequest
            {
                InstitutionId = "ins_109508",
                InitialProducts = new[] { Products.Auth, Products.Identity, Products.Transactions }
            });

            if (!response.IsSuccessStatusCode)
                throw new ApplicationException(response.Error?.DisplayMessage ?? "Failed to create full sandbox public token.");

            return response.PublicToken;
        }

    }
}
