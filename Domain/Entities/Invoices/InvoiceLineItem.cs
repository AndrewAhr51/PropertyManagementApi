using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class InvoiceLineItem
    {
        public int LineItemId { get; set; }
        public int InvoiceId { get; set; }
        public int LineItemTypeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }

        // 🔧 Optional properties for enhanced control
        public int SortOrder { get; set; } = 0;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedDate { get; set; } = DateTime.UtcNow;

        public Invoice Invoice { get; set; } = null!;
        public lkupLineItemType InvoiceType { get; set; } = null!;
        public ICollection<InvoiceLineItemMetadata> Metadata { get; set; } = new List<InvoiceLineItemMetadata>();

    }
}