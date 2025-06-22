namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class CleaningFeeInvoiceCreateDto :InvoiceDto
    {
        public string CleaningType { get; set; } = string.Empty;
    }

}
