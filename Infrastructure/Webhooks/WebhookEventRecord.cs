using System;

namespace PropertyManagementAPI.Infrastructure.Webhooks
{
    public class WebhookEventRecord
    {
        public string EventId { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string Outcome { get; set; } = "Received"; // e.g., "Success", "Error", "Skipped"
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
        public string ErrorDetails { get; set; } = string.Empty;
    }
}


