using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class RentInvoiceCreateDto: InvoiceDto
    {  

        [Required]
        [Range(1, 12, ErrorMessage = "RentMonth must be between 1 and 12.")]
        public int RentMonth { get; set; }

        [Required]
        [Range(2000, 2100, ErrorMessage = "RentYear must be between 2000 and 2100.")]
        public int RentYear { get; set; }
        
    }
}
