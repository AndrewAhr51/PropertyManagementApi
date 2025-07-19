namespace PropertyManagementAPI.Domain.DTOs.Payments.Stripe
{
    public class StripeSessionDto
    {
        public string SessionId { get; set; } = string.Empty;
        public long AmountTotal { get; set; } // in cents
        public string Currency { get; set; } = "USD";
        public StripeSessionMetadata Metadata { get; set; } = new();
    }

    public class StripeSessionMetadata
    {
        public string InvoiceId { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string PropertyId { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
    }
}