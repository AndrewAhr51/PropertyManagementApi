using PropertyManagementAPI.Domain.DTOs.Payments.PreferredMethods;

namespace PropertyManagementAPI.Application.Services.Payments.PreferredMethods
{
    public interface IPreferredMethodService
    {
        Task<int> SetPreferredMethodAsync(CreatePreferredMethodDto dto);
        Task<PreferredMethodResponseDto> GetPreferredMethodByTenantAsync(int tenantId);
        Task<PreferredMethodResponseDto> GetPreferredMethodByOwnerAsync(int ownerId);
    }
}
