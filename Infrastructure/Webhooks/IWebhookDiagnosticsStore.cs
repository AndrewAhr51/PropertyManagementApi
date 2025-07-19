namespace PropertyManagementAPI.Infrastructure.Webhooks
{
    public interface IWebhookDiagnosticsStore
    {
        void Log(WebhookEventRecord record);

        List<WebhookEventRecord> GetBySessionId(string sessionId);

        WebhookEventRecord? GetByEventId(string eventId);

        List<WebhookEventRecord> GetAll();
    }
}