using PropertyManagementAPI.Common.Helpers;

namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public string CustomerName { get; set; } = "Default Customer";
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public int PropertyId { get; set; }
        public string ReferenceNumber { get; set; } 
        public bool IsPaid { get; set; } = false;
        public string Status { get; set; } = "Pending";
        public string? Notes { get; set; }
        public int InvoiceTypeId { get; set; }
        public string CreatedBy { get; set; } = "Web";
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
