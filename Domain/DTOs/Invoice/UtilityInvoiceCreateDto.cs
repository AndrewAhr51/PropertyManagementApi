namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class UtilityInvoiceCreateDto
    {
        public int InvoiceId { get; set; }
        public string UtilityType { get; set; } = string.Empty;
        public decimal UsageAmount { get; set; }
    }

}
