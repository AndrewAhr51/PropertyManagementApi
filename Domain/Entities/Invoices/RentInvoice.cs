namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class RentInvoice:Invoice
    {
        public int RentMonth { get; set; }
        public int RentYear { get; set; }
    }

}
