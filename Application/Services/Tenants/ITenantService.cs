using PropertyManagementAPI.Domain.DTOs.Users;

public interface ITenantService
{
    Task<TenantDto> CreateTenantsAsync(TenantDto dto);
    Task<IEnumerable<TenantDto>> GetAllTenantsAsync();
    Task<TenantDto?> GetTenantByIdAsync(int tenantId);
    Task<bool> UpdateTenantAsync(int tenantId, TenantDto dto);
    Task<bool> SetActivateTenant(int tenantId);
}