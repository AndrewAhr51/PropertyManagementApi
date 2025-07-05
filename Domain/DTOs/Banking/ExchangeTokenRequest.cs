namespace PropertyManagementAPI.Domain.DTOs.Banking
{
    public class ExchangeTokenRequest
    {
        public string PublicToken { get; set; } = null!;

        public int PropertyId { get; set; }
        public int TenantId { get; set; }
    }
}

