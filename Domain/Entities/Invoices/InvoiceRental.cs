namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class InvoiceRental:Invoice
    {
        public int RentMonth { get; set; }
        public int RentYear { get; set; }
    }

}
