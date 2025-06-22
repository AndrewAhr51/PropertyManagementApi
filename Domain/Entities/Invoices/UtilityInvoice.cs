namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class UtilityInvoice : Invoice
    {
        public string UtilityType { get; set; } = default!;
        public decimal UsageAmount { get; set; }
    }

}
