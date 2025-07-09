using PropertyManagementAPI.Domain.DTOs.Quickbooks;

namespace PropertyManagementAPI.Infrastructure.Repositories.Quickbooks
{
    public interface ITokenStore
    {
        Task<TokenSet?> GetTokenAsync(string realmId);
        Task SaveTokenAsync(string realmId, TokenSet token);
        Task DeleteTokenAsync(string realmId);
    }
}
