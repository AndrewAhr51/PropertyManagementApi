namespace PropertyManagementAPI.Application.Services.Quickbooks
{
    public interface IStateManager
    {
        string GenerateStateFromTenantId(int tenantId);
        int ResolveTenantIdFromState(string state);
    }


}
