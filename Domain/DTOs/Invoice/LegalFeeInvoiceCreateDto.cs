using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class LegalFeeInvoiceCreateDto : InvoiceCreateDto
    {
        [Required]
        public string CaseReference { get; set; } = string.Empty;

        [Required]
        public string LawFirm { get; set; } = string.Empty;
    }
}
