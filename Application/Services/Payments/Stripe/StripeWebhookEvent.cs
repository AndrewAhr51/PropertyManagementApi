namespace PropertyManagementAPI.Application.Services.Payments.Stripe
{
    public class StripeWebhookEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string StripeEventId { get; set; }
        public string EventType { get; set; }
        public string Payload { get; set; }
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
        public bool Processed { get; set; } = false;
        public string ErrorMessage { get; set; }
    }
}
