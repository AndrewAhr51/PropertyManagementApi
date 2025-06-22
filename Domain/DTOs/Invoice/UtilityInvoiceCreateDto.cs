using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class UtilityInvoiceCreateDto : InvoiceDto
    {
        [Required]
        public string UtilityType { get; set; } = string.Empty;
        [Required]
        public decimal UsageAmount { get; set; }
    }
}
