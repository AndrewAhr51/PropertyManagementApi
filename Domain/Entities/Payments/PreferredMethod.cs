using global::PropertyManagementAPI.Domain.Entities.User;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities.Payments
{
    public class PreferredMethod
    {
        [Key]
        public int PreferredMethodId { get; set; }

        public int? TenantId { get; set; }
        public int? OwnerId { get; set; }

        [Required]
        [MaxLength(20)]
        public string MethodType { get; set; } // e.g., "Card", "Bank"

        public int? CardTokenId { get; set; }
        public int? BankAccountInfoId { get; set; }

        public bool IsDefault { get; set; } = false;
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;

        // 🔗 Navigation properties
        public CardToken CardToken { get; set; }
        public BankAccountInfo BankAccountInfo { get; set; }

        public Tenant Tenant { get; set; }
        public Owner Owner { get; set; }
    }
}

