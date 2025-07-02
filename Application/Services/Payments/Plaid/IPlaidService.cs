namespace PropertyManagementAPI.Application.Services.Payments.Plaid
{
    public interface IPlaidService
    {
        Task<string> CreateLinkTokenAsync();
        Task<string> ExchangePublicTokenAsync(string publicToken);
        Task<IEnumerable<Going.Plaid.Entity.Account>>  GetBankAccountsAsync(string accessToken);
    }

}
