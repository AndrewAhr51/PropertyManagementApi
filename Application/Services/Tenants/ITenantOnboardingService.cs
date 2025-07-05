namespace PropertyManagementAPI.Application.Services.Tenants
{
    public interface ITenantOnboardingService
    {
        Task OnPlaidAccountVerified(int propertuyId, int tenantId);
    }
}
