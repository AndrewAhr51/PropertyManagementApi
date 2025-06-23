namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class LegalFeeInvoice : Invoice
    {
        public string CaseReference { get; set; } = default!;
        public string LawFirm { get; set; } = default!;
    }

}
