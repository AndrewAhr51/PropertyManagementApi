using PropertyManagementAPI.Infrastructure.Repositories.Tenants;

namespace PropertyManagementAPI.Application.Services.Quickbooks
{
    public class QuickBooksTenantLinker : IQuickBooksTenantLinker
    {
        private readonly ITenantRepository _tenantRepository;

        public QuickBooksTenantLinker(ITenantRepository tenantRepository)
        {
            _tenantRepository = tenantRepository;
        }

        public async Task LinkTenantAccountAsync(int tenantId, string accessToken, string refreshToken, string realmId)
        {
            var tenant = await _tenantRepository.GetTenantByIdAsync(tenantId);
            if (tenant is null) throw new Exception("Tenant not found.");

            tenant.QuickBooksAccessToken = accessToken;
            tenant.QuickBooksRefreshToken = refreshToken;
            tenant.RealmId = realmId;

            await _tenantRepository.UpdateTenantAsync(tenant);
        }
    }
}
