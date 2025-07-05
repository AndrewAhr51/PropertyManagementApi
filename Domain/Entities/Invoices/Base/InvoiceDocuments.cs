using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities.Invoices.Base
{
    public abstract class InvoiceDocuments
    {
        [Key]
        public int InvoiceId { get; set; }
        public int TenantId { get; set; }
        public string TenantName { get; set; } = "unknown";
        public string Email { get; set; } = default!;
        public string ReferenceNumber { get; set; } = default!;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsPaid { get; set; } = false;
        public string Status { get; set; } = "Pending";
        public string CreatedBy { get; set; } = "Web";
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();

        [NotMapped]
        public decimal ComputedAmount => LineItems?.Sum(li => li.Amount) ?? 0m;

    }

}