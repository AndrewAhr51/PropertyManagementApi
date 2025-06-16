using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities;
public class Invoice
{
    [Key]
    public int InvoiceId { get; set; }

    [Required]
    public int TenantId { get; set; }

    [Required]
    public int PropertyId { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal AmountDue { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    [MaxLength(20)]
    public string BillingPeriod { get; set; } = "Monthly";

    [Required]
    [MaxLength(10)]
    public string BillingMonth { get; set; } = DateTime.Now.ToString("MMMM");

    [Column(TypeName = "decimal(10,2)")]
    public decimal LateFee { get; set; } = 0;

    [Column(TypeName = "decimal(10,2)")]
    public decimal DiscountsApplied { get; set; } = 0;

    public bool IsPaid { get; set; } = false;

    public DateTime? PaymentDate { get; set; }

    [MaxLength(50)]
    public string PaymentMethod { get; set; }

    [MaxLength(100)]
    public string PaymentReference { get; set; }

    [Required]
    [MaxLength(20)]
    public string InvoiceStatus { get; set; } = "Pending";

    [Required]
    public int InvoiceTypeId { get; set; } // Changed to reference lookup table

    [MaxLength(50)]
    public string GeneratedBy { get; set; } = "Web";

    [MaxLength(500)]
    public string Notes { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}