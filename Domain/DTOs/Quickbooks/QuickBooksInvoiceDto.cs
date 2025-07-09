namespace PropertyManagementAPI.Domain.DTOs.Quickbooks
{
    public class QuickBooksInvoiceDto
    {
        public string Id { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string SyncStatus { get; set; } = "Pending";
    }
}