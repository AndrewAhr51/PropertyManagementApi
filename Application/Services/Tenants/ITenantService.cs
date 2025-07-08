using PropertyManagementAPI.Domain.DTOs.Users;

namespace PropertyManagementAPI.Application.Services.Tenants
{
    public interface ITenantService
    {
        Task<TenantDto> CreateTenantsAsync(TenantDto dto);
        Task<IEnumerable<TenantDto>> GetAllTenantsAsync();
        Task<TenantDto?> GetTenantByIdAsync(int tenantId);
        Task<List<TenantDto?>> GetTenantByPropertyIdAsync(int tenantId);
        Task<bool> UpdateTenantAsync(int tenantId, TenantDto dto);
        Task<bool> SetActivateTenant(int tenantId);
        Task<bool>LinkQuickBooksAccountAsync(int tenantId, string accessToken, string refreshToken, string realmId);
    }
}