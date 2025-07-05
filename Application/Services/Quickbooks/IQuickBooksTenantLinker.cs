namespace PropertyManagementAPI.Application.Services.Quickbooks
{
    public interface IQuickBooksTenantLinker
    {
        Task LinkTenantAccountAsync(int tenantId, string accessToken, string refreshToken, string realmId);
    }

}
