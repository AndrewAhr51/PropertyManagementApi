using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class InvoiceRentCreateDto
    {
        [Required]
        public int InvoiceId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime DueDate { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required]
        public int InvoiceTypeId { get; set; }

        [Required]
        [Range(1, 12, ErrorMessage = "RentMonth must be between 1 and 12.")]
        public int RentMonth { get; set; }

        [Required]
        [Range(2000, 2100, ErrorMessage = "RentYear must be between 2000 and 2100.")]
        public int RentYear { get; set; }

        [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? Notes { get; set; }
    }
}
