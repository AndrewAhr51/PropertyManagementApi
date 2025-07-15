namespace PropertyManagementAPI.Domain.DTOs.Invoices
{
    public class OpenInvoiceByTenantDto
    {
        public int TenantId { get; set; }
        public string TenantName { get; set; }
        public int TotalInvoices { get; set; }
        public List<InvoiceDto> Invoices { get; set; } = new();
    }

}
