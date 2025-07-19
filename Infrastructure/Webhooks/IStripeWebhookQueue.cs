using PropertyManagementAPI.Domain.DTOs.Stripe;
using Stripe;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PropertyManagementAPI.Infrastructure.Webhooks
{
    public interface IStripeWebhookQueue
    {
        /// <summary>
        /// Enqueue a Stripe event and its raw JSON payload.
        /// </summary>
        Task EnqueueAsync(Event stripeEvent, string rawJson);

        /// <summary>
        /// Try to dequeue an event in a polling-style consumer.
        /// </summary>
        bool TryDequeue(out (Event StripeEvent, string RawJson) item);

        /// <summary>
        /// Stream queued events asynchronously — preferred for background processors.
        /// </summary>
        IAsyncEnumerable<(Event StripeEvent, string RawJson)> ReadEventsAsync(CancellationToken cancellationToken);

        StripeQueueHealthDto GetHealth();
    }
}