namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class PropertyTaxInvoiceCreateDto
    {
        public int InvoiceId { get; set; }
        public DateTime TaxPeriodStart { get; set; }
        public DateTime TaxPeriodEnd { get; set; }
    }

}
