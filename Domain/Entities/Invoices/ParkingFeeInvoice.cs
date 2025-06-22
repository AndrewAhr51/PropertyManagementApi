namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class ParkingFeeInvoice : Invoice
    {
        public string SpotIdentifier { get; set; } = default!;
    }
}
