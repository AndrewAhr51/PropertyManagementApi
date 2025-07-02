namespace PropertyManagementAPI.Application.Services.Payments.Plaid
{
    public interface IPlaidSandboxTestService
    {
        Task<string> CreateSandboxPublicTokenAsync();
        Task<string> CreateFullSandboxPublicTokenAsync();
    }

}
