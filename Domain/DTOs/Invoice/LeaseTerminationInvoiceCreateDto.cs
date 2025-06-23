using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class LeaseTerminationInvoiceCreateDto : InvoiceDto
    {
        [Required]
        public string TerminationReason { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }
    }

}
