namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class ParkingFeeInvoiceCreateDto
    {
        public int InvoiceId { get; set; }
        public string SpotIdentifier { get; set; } = string.Empty;
    }

}
