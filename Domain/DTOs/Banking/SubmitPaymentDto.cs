using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Banking
{
    public class SubmitPaymentDto
    {
        [Required, Range(1, 100000)]
        public decimal Amount { get; set; }

        [Required]
        public string StripePaymentIntentId { get; set; }
    }

}
