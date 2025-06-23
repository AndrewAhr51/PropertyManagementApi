using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class CleaningFeeInvoiceCreateDto :InvoiceDto
    {
        [Required]
        public string CleaningType { get; set; } = string.Empty;

    }

}
