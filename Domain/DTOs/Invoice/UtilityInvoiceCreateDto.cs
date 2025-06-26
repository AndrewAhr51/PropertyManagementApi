using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class UtilityInvoiceCreateDto : InvoiceCreateDto
    {
        [Required]
        public string UtilityType { get; set; } = string.Empty;
        [Required]
        public decimal UsageAmount { get; set; }
    }
}
