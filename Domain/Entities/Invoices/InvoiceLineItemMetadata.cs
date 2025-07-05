namespace PropertyManagementAPI.Domain.Entities.Invoices
{
    public class InvoiceLineItemMetadata
    {
        public int LineItemId { get; set; }
        public string MetaKey { get; set; } = string.Empty;
        public string MetaValue { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // ✅ Add this line

        public InvoiceLineItem LineItem { get; set; } = null!;
    }
}
