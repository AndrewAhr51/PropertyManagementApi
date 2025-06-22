namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class CleaningFeeInvoice : Invoice
    {
        public string CleaningType { get; set; } = default!;
    }

}
