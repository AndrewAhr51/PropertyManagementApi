using PropertyManagementAPI.Domain.DTOs.Payments;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public interface IPreferredMethodService
    {
        Task<int> SetPreferredMethodAsync(CreatePreferredMethodDto dto);
        Task<PreferredMethodResponseDto> GetPreferredMethodByTenantAsync(int tenantId);
        Task<PreferredMethodResponseDto> GetPreferredMethodByOwnerAsync(int ownerId);
    }
}
