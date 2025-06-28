namespace PropertyManagementAPI.Domain.DTOs.Payments
{
    public class CreateCardTokenDto
    {
        public string TokenValue { get; set; }
        public string CardBrand { get; set; }
        public string Last4Digits { get; set; }
        public DateTime Expiration { get; set; }
        public int TenantId { get; set; }
        public int OwnerId { get; set; }
        public bool IsDefault { get; set; }
    }


}
