namespace PropertyManagementAPI.Domain.DTOs.Invoices
{
    public class CreateInvoiceDto
    {
        public int PropertyId { get; set; }
        public DateTime DueDate { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
