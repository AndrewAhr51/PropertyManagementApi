namespace PropertyManagementAPI.Domain.DTOs.Invoices
{
    public class CreateInvoiceLineItemDto
    {
        public int InvoiceId { get; set; }
        public string LineItemTypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public int? SortOrder { get; set; } // ✅ Add this
        public List<InvoiceLineItemMetadataDto>? Metadata { get; set; }
    }

}
