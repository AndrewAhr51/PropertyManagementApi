namespace PropertyManagementAPI.Domain.DTOs;
public class InvoiceDto
{
    public int InvoiceId { get; set; }
    public int TenantId { get; set; }
    public int PropertyId { get; set; }
    public decimal AmountDue { get; set; }
    public DateTime DueDate { get; set; }
    public string BillingPeriod { get; set; } = "Monthly";
    public string BillingMonth { get; set; } = DateTime.Now.ToString("MMMM");
    public decimal LateFee { get; set; } = 0;
    public decimal DiscountsApplied { get; set; } = 0;
    public bool IsPaid { get; set; } = false;
    public DateTime? PaymentDate { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentReference { get; set; }
    public string InvoiceStatus { get; set; } = "Pending";
    public int InvoiceTypeId { get; set; } // Changed from string to int
    public string GeneratedBy { get; set; } = "Web";
    public string Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}