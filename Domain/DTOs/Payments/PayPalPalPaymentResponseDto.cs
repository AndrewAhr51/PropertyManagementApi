namespace PropertyManagementAPI.Domain.DTOs.Payments
{
    public class PayPalPaymentResponseDto
    {
        public string OrderId { get; set; } = null!;
        public string Status { get; set; } = "CREATED"; // "CREATED", "APPROVED", or "COMPLETED"
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; } = "USD";
        public int InvoiceId { get; set; }
        public string? InvoiceReference { get; set; }

        // Approval phase
        public string? ApprovalLink { get; set; }

        // Capture phase
        public string? GatewayTransactionId { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? Note { get; set; }

        public string PerformedBy { get; set; } = "Web";
    }
}