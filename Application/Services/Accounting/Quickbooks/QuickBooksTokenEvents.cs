namespace PropertyManagementAPI.Application.Services.Accounting.Quickbooks
{
    public class QuickBooksTokenEvents
    {
        public record QuickBooksTokenEvent(string TenantId, string EventType, DateTime Timestamp);
    }
}
