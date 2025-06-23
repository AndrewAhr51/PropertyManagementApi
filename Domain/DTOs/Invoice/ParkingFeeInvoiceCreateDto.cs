using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class ParkingFeeInvoiceCreateDto : InvoiceDto
    {
        [Required]
        public string SpotIdentifier { get; set; } = string.Empty;

    }

}
