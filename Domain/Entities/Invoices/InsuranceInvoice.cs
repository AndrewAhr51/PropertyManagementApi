namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class InsuranceInvoice : Invoice
    {
        public string PolicyNumber { get; set; } = default!;
        public DateTime CoveragePeriodStart { get; set; }
        public DateTime CoveragePeriodEnd { get; set; }
    }

}
