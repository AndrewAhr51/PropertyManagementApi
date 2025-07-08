namespace PropertyManagementAPI.Domain.DTOs.Payments.PayPal
{
    public class CapturePayPalDto
    {
        public string OrderId { get; set; }                  // Order ID returned by PayPal after approval
        public int InvoiceId { get; set; }                   // Invoice being paid
        public int TenantId { get; set; }                    // Who owns the invoice
        public int OwnerId { get; set; }                     // Optional owner context
        public decimal Amount { get; set; }                  // Payment amount
        public DateTime PaymentDate { get; set; }            // When capture was executed
        public string PerformedBy { get; set; }              // Actor label: "Web", "System", or actual user
        public Dictionary<string, string> Metadata { get; set; } = new(); // Optional card metadata

        public CreatePayPalDto ToCreatePayPalDto()
        {
            return new CreatePayPalDto
            {
                InvoiceId = InvoiceId,
                Metadata = Metadata
            };
        }
    }
}
