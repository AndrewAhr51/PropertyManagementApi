using PropertyManagementAPI.Domain.DTOs;

public interface ITenantService
{
    Task<TenantDto> CreateAsync(TenantDto dto);
    Task<IEnumerable<TenantDto>> GetAllAsync();
    Task<TenantDto?> GetByIdAsync(int tenantId);
    Task<bool> UpdateAsync(int tenantId, TenantDto dto);
    Task<bool> DeleteAsync(int tenantId);
}