namespace PropertyManagementAPI.Domain.DTOs.Invoices
{
    public class InvoiceLineItemMetadataDto
    {
        public string MetaKey { get; set; } = default!;
        public string? MetaValue { get; set; }
    }
}