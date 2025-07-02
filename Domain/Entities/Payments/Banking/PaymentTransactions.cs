using PropertyManagementAPI.Domain.Entities.User;

namespace PropertyManagementAPI.Domain.Entities.Payments.Banking
{
    public class PaymentTransactions
    {
        public int PaymentTransactionId { get; set; }
        public int TenantId { get; set; }
        public decimal Amount { get; set; }
        public string StripePaymentIntentId { get; set; }
        public string Status { get; set; } // e.g., "pending", "succeeded", "failed"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Tenant Tenant { get; set; }
    }
}
