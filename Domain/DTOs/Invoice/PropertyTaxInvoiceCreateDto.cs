namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class PropertyTaxInvoiceCreateDto : InvoiceCreateDto
    {
        public DateTime TaxPeriodStart { get; set; }
        public DateTime TaxPeriodEnd { get; set; }
     
    }

}
