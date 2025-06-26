using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class ParkingFeeInvoiceCreateDto : InvoiceCreateDto
    {
        [Required]
        public string SpotIdentifier { get; set; } = string.Empty;

    }

}
