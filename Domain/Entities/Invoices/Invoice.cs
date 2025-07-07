using PropertyManagementAPI.Domain.Entities.Invoices.Base;

namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class Invoice : InvoiceDocuments
    {
        public decimal LastMonthDue { get; set; }
        public decimal LastMonthPaid { get; set; }
        public int RentMonth { get; set; }
        public int RentYear { get; set; }
        public string? Notes { get; set; }

        public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();

    }
}