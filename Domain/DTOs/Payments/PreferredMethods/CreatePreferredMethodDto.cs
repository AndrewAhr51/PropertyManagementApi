namespace PropertyManagementAPI.Domain.DTOs.Payments.PreferredMethods
{
    public class CreatePreferredMethodDto
    {
        public int? TenantId { get; set; }
        public int? OwnerId { get; set; }
        public string MethodType { get; set; }
        public int? CardTokenId { get; set; }
        public int? BankAccountInfoId { get; set; }
        public bool IsDefault { get; set; }
    }

}
