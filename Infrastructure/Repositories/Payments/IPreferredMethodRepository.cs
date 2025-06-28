using PropertyManagementAPI.Domain.Entities.Payments;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments
{
    public interface IPreferredMethodRepository
    {
        Task<PreferredMethod> GetDefaultByTenantAsync(int tenantId);
        Task<PreferredMethod> GetDefaultByOwnerAsync(int ownerId);
        Task<int> UpsertAsync(PreferredMethod method);
        Task ClearDefaultAsync(int? tenantId, int? ownerId);
    }
}
