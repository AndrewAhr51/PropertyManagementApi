namespace PropertyManagementAPI.Domain.Entities.Payments
{
    public class PaymentAuditLog
    {
        public int Id { get; set; }
        public int? PaymentId { get; set; }
        public string Gateway { get; set; } = "PayPal";
        public string Action { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? ResponsePayload { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? PerformedBy { get; set; }

        public Payment? Payment { get; set; }
    }
}
