namespace PropertyManagementAPI.Domain.DTOs.Payments.Stripe
{
    public class StripePaymentResponseDto
    {
        public string ClientSecret { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "usd";
        public int InvoiceId { get; set; }
        public string? InvoiceReference { get; set; }
    }

}
