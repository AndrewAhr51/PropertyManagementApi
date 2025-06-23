namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class PropertyTaxInvoiceCreateDto : InvoiceDto
    {
        public DateTime TaxPeriodStart { get; set; }
        public DateTime TaxPeriodEnd { get; set; }
     
    }

}
