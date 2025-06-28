using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.Entities.Payments;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public interface ICardTokenService
    {
        Task<int> CreateCardTokenAsync(CreateCardTokenDto dto);
        Task<IEnumerable<CardToken>> GetTokensByTenantAsync(int tenantId);
        Task SetDefaultTokenAsync(int cardTokenId, int tenantId);
    }

}
