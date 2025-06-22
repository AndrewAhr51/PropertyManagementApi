namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class LeaseTerminationInvoice : Invoice
    {
        public string TerminationReason { get; set; } = default!;
    }

}
