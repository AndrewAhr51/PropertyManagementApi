namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class CumulativeInvoiceDto
    {
        public int InvoiceId { get; set; }
        public int PropertyId { get; set; } 
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime DueDate { get; set; }
        public string? Notes { get; set; }
        public string? Status { get; set; }
        public string InvoiceType { get; set; } = string.Empty;
    }
}
