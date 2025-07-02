using PropertyManagementAPI.Domain.Entities.Payments.PreferredMethods;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments.PreferredMethods
{
    public interface IPreferredMethodRepository
    {
        Task<PreferredMethod> GetDefaultByTenantAsync(int tenantId);
        Task<PreferredMethod> GetDefaultByOwnerAsync(int ownerId);
        Task<int> UpsertAsync(PreferredMethod method);
        Task ClearDefaultAsync(int? tenantId, int? ownerId);
    }
}
