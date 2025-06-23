using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class CleaningFeeInvoiceCreateDto :InvoiceDto
    {
        [Required]
        public string CleaningType { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

    }

}
