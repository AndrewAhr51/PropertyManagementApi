using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class InvoiceDto
    {
        public int InvoiceId { get; set; }
        public string CustomerName { get; set; } = "unknown";
        public string ReferenceNumber { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; } = null!;
        public bool IsPaid { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Notes { get; set; }
        public int InvoiceTypeId { get; set; }
        public string InvoiceTypeName { get; set; } = null!;
        public string CreatedBy { get; set; } = "Web";
        public DateTime CreatedDate { get; set; }
    }

}
