namespace PropertyManagementAPI.Application.Configuration
{
    public class PlaidSettings
    {
        public string ClientId { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
        public string Environment { get; set; } = "sandbox";
    }
}
