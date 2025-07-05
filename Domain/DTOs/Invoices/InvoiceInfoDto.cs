namespace PropertyManagementAPI.Domain.DTOs.Invoices
{
    public class InvoiceInfoDto
    {
        public decimal LastMonthDue { get; set; }
        public decimal LastMonthPaid { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal Amount { get; set; }

    }
}
