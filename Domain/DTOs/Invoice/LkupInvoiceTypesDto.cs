namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class LkupInvoiceTypeDto
    {
        public int InvoiceTypeId { get; set; }
        public string InvoiceType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = "Web";
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
