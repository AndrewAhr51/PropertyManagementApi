using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities.Payments
{
    public class PaymentMethods
    {
        [Key]
        public int PreferredMethodId { get; set; }

        // Party association (only one should be set)
        public int? TenantId { get; set; }
        public int? OwnerId { get; set; }

        // Type of payment method
        public string MethodType { get; set; } // e.g., "Card", "Bank", "Check"

        // References to tokenized/card or bank info
        public int? CardTokenId { get; set; }
        public int? BankAccountInfoId { get; set; }

        public bool IsDefault { get; set; }
        public DateTime UpdatedOn { get; set; }

    }
}