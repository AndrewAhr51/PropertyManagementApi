namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class LegalFeeInvoiceCreateDto : InvoiceDto
    {
        public string CaseReference { get; set; } = string.Empty;
    }

}
