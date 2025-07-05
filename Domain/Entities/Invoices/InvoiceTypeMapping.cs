using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class InvoiceTypeMapping
    {
        [Column(Order = 0)]
        public int InvoiceId { get; set; }

        [Column(Order = 1)]
        public int InvoiceTypeId { get; set; }

        public Invoice Invoice { get; set; } = default!;
        public lkupLineItemType InvoiceType { get; set; } = default!;
        public int LineItemTypeId { get; internal set; }
    }

}
