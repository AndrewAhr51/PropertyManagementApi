using Microsoft.VisualBasic;

namespace PropertyManagementAPI.Domain.DTOs.Payments
{
    public class CreateStripeDto
    {
        public int InvoiceId { get; set; }
        public int PropertyId { get; set; }
        public int? TenantId { get; set; }
        public int? OwnerId { get; set; }
        public DateTime PaidOn { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string PaymentMethod { get; set; } = "Card"; // default to card payment
        public Dictionary<string, string> Metadata { get; set; } = new(); // optional, initialized to avoid nulls
    }

}
