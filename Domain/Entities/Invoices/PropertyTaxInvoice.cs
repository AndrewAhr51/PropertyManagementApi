namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class PropertyTaxInvoice : Invoice
    {
        public DateTime TaxPeriodStart { get; set; }
        public DateTime TaxPeriodEnd { get; set; }
    }

}
