using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    [Required]
    public int TenantId { get; set; } = 0;

    public bool IsPaid { get; set; } = false;

    [MaxLength(50)]
    public string Status { get; set; } = "Pending";

    public string? Notes { get; set; }

    [Required]
    public int InvoiceTypeId { get; set; }

    [MaxLength(50)]
    public string CreatedBy { get; set; } = "Web";

    public DateTime CreatedDate { get; set; }
}