namespace PropertyManagementAPI.Domain.DTOs.Payments.Stripe
{
    public class StripeSessionDto
    {
        public string SessionId { get; set; } = string.Empty;
        public long AmountTotal { get; set; } // in cents
        public string Currency { get; set; } = "USD";

        public string ReceiptUrl { get; set; } = string.Empty;
        public string CardBrand { get; set; } = string.Empty;
        public string CardLast4 { get; set; } = string.Empty;
        public string ExpMonth { get; set; } = string.Empty;
        public string ExpYear { get; set; } = string.Empty;
        public string ChargeStatus { get; set; } = string.Empty;
        public string StripeChargeId { get; set; } = string.Empty;

        public StripeSessionMetadata Metadata { get; set; } = new();
        public StripeBillingAddress Address { get; set; } = new();
    }

    public class StripeSessionMetadata
    {
        public string InvoiceId { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        public string PropertyId { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
    }

    public class StripeBillingAddress
    {
        public string Line1 { get; set; } = string.Empty;
        public string Line2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}