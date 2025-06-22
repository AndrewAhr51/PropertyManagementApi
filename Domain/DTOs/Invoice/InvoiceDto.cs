using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class InvoiceDto
    {
        [Required]
        public int InvoiceId { get; set; }

        [Required]
        public string InvoiceType { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required]
        public DateTime DueDate { get; set; } = DateTime.Now;

        [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? Notes { get; set; }
    }
}
