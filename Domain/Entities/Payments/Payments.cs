using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Domain.Entities.Payments
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int InvoiceId { get; set; }
        public int TenantId { get; set; }
        public int PropertyId { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethodId { get; set; }
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string ReferenceNumber { get; set; } = null!;
        public string CreatedBy { get; set; } = "Web";
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    }

}
