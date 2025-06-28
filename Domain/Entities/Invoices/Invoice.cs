using PropertyManagementAPI.Domain.Entities.Payments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = "unknown";

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = "unknown";

        [Required]
        [MaxLength(50)]
        public string ReferenceNumber { get; set; } = null!;

        [Required]
        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public int PropertyId { get; set; }

        public int? TenantId { get; set; }

        public int? OwnerId { get; set; }

        public bool IsPaid { get; set; } = false;

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        public string? Notes { get; set; }

        [Required]
        public int InvoiceTypeId { get; set; }

        [MaxLength(50)]
        public string CreatedBy { get; set; } = "Web";

        public DateTime CreatedDate { get; set; }

        // 🔗 Navigation property for related payments
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

    }
}