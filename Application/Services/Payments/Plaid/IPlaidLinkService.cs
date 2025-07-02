namespace PropertyManagementAPI.Application.Services.Payments.Plaid
{
    public interface IPlaidLinkService
    {
        Task<string> CreateLinkTokenAsync();
    }
}
