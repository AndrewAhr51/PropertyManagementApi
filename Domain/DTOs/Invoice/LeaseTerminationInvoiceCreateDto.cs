using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class LeaseTerminationInvoiceCreateDto : InvoiceCreateDto
    {
        [Required]
        public string TerminationReason { get; set; } = string.Empty;

    }

}
