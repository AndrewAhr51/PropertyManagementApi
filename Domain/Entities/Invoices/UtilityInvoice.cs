namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class UtilityInvoice : Invoice
    {
        public int UtilityTypeId { get; set; }
        public decimal UsageAmount { get; set; }
    }

}
