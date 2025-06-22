namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class CleaningFeeInvoiceCreateDto
    {
        public int InvoiceId { get; set; }
        public string CleaningType { get; set; } = string.Empty;
    }

}
