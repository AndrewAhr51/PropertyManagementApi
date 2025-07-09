namespace PropertyManagementAPI.Domain.Entities.Payments.Quickbooks
{
    public class QuickBooksAuditLog
    {
        public int Id { get; set; }
        public int TenantId { get; set; }
        public string RealmId { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string? CorrelationId { get; set; }
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    }
}
