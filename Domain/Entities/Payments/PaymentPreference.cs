using PropertyManagementAPI.Domain.Entities.User;

namespace PropertyManagementAPI.Domain.Entities.Payments
{
    public class PaymentPreference
    {
        public int PaymentPreferenceId { get; set; }

        public int? TenantId { get; set; }
        public Tenant Tenant { get; set; }

        public int? OwnerId { get; set; }
        public Owner Owner { get; set; }

        public string PreferredMethod { get; set; } // "Card", "Check", "Transfer"

        public Guid? CardTokenId { get; set; }
        public CardToken CardToken { get; set; }

        public int? BankAccountInfoId { get; set; }
        public BankAccountInfo BankAccountInfo { get; set; }

        public bool IsDefault { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
