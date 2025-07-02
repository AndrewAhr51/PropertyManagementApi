using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments.CardTokens
{
    public interface ICardTokenRepository
    {
        Task<int> AddCardTokenAsync(CardToken token);
        Task<CardToken> GetCardTokenByIdAsync(int cardTokenId);
        Task<IEnumerable<CardToken>> GetCardTokenByTenantAsync(int tenantId);
        Task SetCardTokenDefaultAsync(int cardTokenId, int tenantId);
        Task ClearCardTokenDefaultAsync(int tenantId, int ownerId);
    }
}
