namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class ParkingFeeInvoiceCreateDto : InvoiceDto
    {
        public string SpotIdentifier { get; set; } = string.Empty;
    }

}
