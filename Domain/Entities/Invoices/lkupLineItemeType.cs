using System.ComponentModel.DataAnnotations;
namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class lkupLineItemType
    {
        [Key]
        public int LineItemTypeId { get; set; }

        public string LineItemTypeName { get; set; } = string.Empty;

        // 🔗 Still used for InvoiceLineItems (FK relationship)
        public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
    }
}
