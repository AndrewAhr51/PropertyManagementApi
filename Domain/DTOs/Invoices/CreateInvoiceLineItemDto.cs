namespace PropertyManagementAPI.Domain.DTOs.Invoices
{
    public class CreateInvoiceLineItemDto
    {
        public int invoiceId { get; set; }
        public string lineItemTypeName { get; set; } = string.Empty;
        public string? description { get; set; }
        public decimal amount { get; set; }
        public int? sortOrder { get; set; } // ✅ Add this
        public List<InvoiceLineItemMetadataDto>? Metadata { get; set; }
    }

}
