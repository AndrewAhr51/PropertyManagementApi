using System.Text;

namespace PropertyManagementAPI.Application.Services.Quickbooks
{
    public class StateManager : IStateManager
    {
        public string GenerateStateFromTenantId(int tenantId)
        {
            var bytes = Encoding.UTF8.GetBytes(tenantId.ToString());
            return Convert.ToBase64String(bytes);
        }

        public int ResolveTenantIdFromState(string state)
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(state));
            return int.Parse(decoded);
        }
    }
}

