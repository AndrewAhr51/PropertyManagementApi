using PropertyManagementAPI.Domain.DTOs.Users;

public interface ITenantRepository
{
    Task<TenantDto> AddAsync(TenantDto dto);
    Task<IEnumerable<TenantDto>> GetAllAsync();
    Task<TenantDto?> GetByIdAsync(int tenantId);
    Task<bool> UpdateAsync(int tenantId, TenantDto dto);
    Task<bool> DeleteAsync(int tenantId);
}