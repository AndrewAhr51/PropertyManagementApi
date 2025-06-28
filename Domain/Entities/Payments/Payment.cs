using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Domain.Entities.User;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities.Payments
{
    public abstract class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentType { get; set; } = null!; // e.g., "Card", "Check", "Transfer"

        public string CardType { get; set; } = null!; // e.g., "Visa", "MasterCard", etc. (only for Card payments)

        public string CheckNumber { get; set; } = null!; // Check number (only for Check payments)

        public string CheckBankName { get; set; } = null!; // Check number (only for Check payments)

        public string BankAccountNumber { get; set; } = null!; // Bank account number (only for Transfer payments)

        public string RoutingNumber { get; set; } = null!; // Routing number (only for Transfer payments)

        public string TransactionId { get; set; } = null!; // Transaction ID (only for Transfer payments)


        public DateTime PaidOn { get; set; }

        [Required]
        [MaxLength(50)]
        public string ReferenceNumber { get; set; } = null!;

        [ForeignKey(nameof(Invoice))]
        public int InvoiceId { get; set; }
        public int? TenantId { get; set; }
        public int? OwnerId { get; set; }

        // 🔗 Navigation properties
        public Invoice Invoice { get; set; } = null!;
        public Tenant Tenant { get; set; } = null!;
        public Owner Owner { get; set; } = null!;
    }
}


