using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class LeaseTerminationInvoiceCreateDto : InvoiceDto
    {
        [Required]
        public string TerminationReason { get; set; } = string.Empty;

    }

}
