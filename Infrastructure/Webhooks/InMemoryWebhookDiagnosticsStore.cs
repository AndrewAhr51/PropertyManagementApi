using System.Collections.Concurrent;
using PropertyManagementAPI.Infrastructure.Webhooks;

namespace PropertyManagementAPI.Infrastructure.Webhooks
{
    public class InMemoryWebhookDiagnosticsStore : IWebhookDiagnosticsStore
    {
        private readonly ConcurrentBag<WebhookEventRecord> _events = new();

        public void Log(WebhookEventRecord record)
        {
            if (record != null)
            {
                _events.Add(record);
            }
        }

        public List<WebhookEventRecord> GetBySessionId(string sessionId)
        {
            return _events
                .Where(e => e.SessionId == sessionId)
                .OrderByDescending(e => e.ReceivedAt)
                .ToList();
        }

        public WebhookEventRecord? GetByEventId(string eventId)
        {
            return _events.FirstOrDefault(e => e.EventId == eventId);
        }

        public List<WebhookEventRecord> GetAll()
        {
            return _events
                .OrderByDescending(e => e.ReceivedAt)
                .ToList();
        }
    }
}