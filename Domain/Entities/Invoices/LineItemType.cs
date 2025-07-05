using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class LineItemType
    {
        public int LineItemTypeId { get; set; }        // Unique ID for the line item
        public int InvoiceId { get; set; }             // Foreign key to the invoice
        public string? Description { get; set; }       // Optional description
        public decimal Amount { get; set; }            // Line item cost

        // Foreign key to InvoiceType
        public int InvoiceTypeId { get; set; }

        // Navigation property to the type descriptor
        public InvoiceType InvoiceType { get; set; } = null!;

        // Optional metadata entries
        public List<LineItemMetadata> Metadata { get; set; } = new();
    }
}