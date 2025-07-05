namespace PropertyManagementAPI.Domain.DTOs.Invoices
{
    public class InvoiceLineItemDto
    {
        public int LineItemId { get; set; }
        public int InvoiceId { get; set; }
        public int? LineItemTypeId { get; set; }
        public string LineItemTypeName { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }

        public List<InvoiceLineItemMetadataDto> Metadata { get; set; } = new();
    }
}

