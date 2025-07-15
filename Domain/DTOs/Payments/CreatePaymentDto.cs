using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Payments
{
  public class CreatePaymentDto
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment date is required.")]
        public DateTime PaidOn { get; set; }

        [Required(ErrorMessage = "Invoice ID is required.")]
        public int InvoiceId { get; set; }

        public int? TenantId { get; set; }

        public int? OwnerId { get; set; }

        public string Currency { get; set; } = "USD"; // default to USD, can be overridden  

        [Required(ErrorMessage = "Payment method is required.")]
        [RegularExpression("^(Card|Check|Transfer)$", ErrorMessage = "Invalid payment method.")]
        public string PaymentMethod { get; set; }

        public Dictionary<string, string> Metadata { get; set; } = new(); // optional, initialized to avoid nulls
    }
}
