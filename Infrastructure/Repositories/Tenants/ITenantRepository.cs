using PropertyManagementAPI.Domain.DTOs.Users;

namespace PropertyManagementAPI.Infrastructure.Repositories.Tenants
{
    public interface ITenantRepository
    {
        Task<TenantDto> AddTenantAsync(TenantDto dto);
        Task<IEnumerable<TenantDto>> GetAllTenantsAsync();
        Task<TenantDto?> GetTenantByIdAsync(int tenantId);
        Task<List<TenantDto?>> GetTenantByPropertyIdAsync(int propertyId);
        Task<bool> UpdateTenantAsync(TenantDto dto);
        Task<bool> SetActivateTenant(int tenantId);
        Task<bool> LinkQuickBooksAccountAsync(int tenantId, string AccessToken, string RefreshToken, string realmId);
        Task<bool> RecordQuickBooksAuditAsync(int tenantId, string realmId, string eventType, string? correlationId = null);
    }
}