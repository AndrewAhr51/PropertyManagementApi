using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class InvoiceCreateDto1
    {
        public int InvoiceId { get; set; }
        public string? CustomerName { get; set; } = "unknown";
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public int PropertyId { get; set; }
        public string? Status { get; set; } = "Pending";
        public string? Notes { get; set; }
        public string InvoiceType { get; set; } = string.Empty;
        public string? CreatedBy { get; set; } = "Web";
    }
}
