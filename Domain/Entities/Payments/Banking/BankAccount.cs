using PropertyManagementAPI.Domain.Entities.User;

namespace PropertyManagementAPI.Domain.Entities.Payments.Banking
{
    public class BankAccount
    {
        public int BankAccountId { get; set; }
        public int TenantId { get; set; }
        public string StripeBankAccountId { get; set; } // Tokenized ID from Stripe
        public string BankName { get; set; }
        public string Last4 { get; set; }
        public string AccountType { get; set; } // e.g., "checking"
        public bool IsVerified { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        public Tenant Tenant { get; set; }
    }
}
