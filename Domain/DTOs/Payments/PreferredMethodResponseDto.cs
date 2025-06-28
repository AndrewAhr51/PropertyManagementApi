namespace PropertyManagementAPI.Domain.DTOs.Payments
{
    public class PreferredMethodResponseDto
    {
        public int PreferredMethodId { get; set; }
        public string MethodType { get; set; }
        public bool IsDefault { get; set; }
        public DateTime UpdatedOn { get; set; }

        public string Last4Digits { get; set; }
        public string CardBrand { get; set; }

        public string BankName { get; set; }
        public string AccountType { get; set; }

        public int? TenantId { get; set; }
        public int? OwnerId { get; set; }
    }

}
