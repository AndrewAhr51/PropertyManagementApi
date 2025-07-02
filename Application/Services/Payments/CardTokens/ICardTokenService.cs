using PropertyManagementAPI.Domain.DTOs.Payments.CardTokens;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;

namespace PropertyManagementAPI.Application.Services.Payments.CardTokens
{
    public interface ICardTokenService
    {
        Task<int> CreateCardTokenAsync(CreateCardTokenDto dto);
        Task<IEnumerable<CardToken>> GetTokensByTenantAsync(int tenantId);
        Task SetDefaultTokenAsync(int cardTokenId, int tenantId);
    }

}
