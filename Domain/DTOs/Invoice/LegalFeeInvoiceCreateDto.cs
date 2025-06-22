namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class LegalFeeInvoiceCreateDto
    {
        public int InvoiceId { get; set; }
        public string CaseReference { get; set; } = string.Empty;
    }

}
