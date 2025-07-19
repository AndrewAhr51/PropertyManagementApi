using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using PropertyManagementAPI.Infrastructure.Webhooks;
using PropertyManagementAPI.Domain.DTOs.Stripe;

namespace PropertyManagementAPI.Application.Services.Payments.Stripe
{
    public class InMemoryStripeWebhookQueue : IStripeWebhookQueue
    {
        private readonly ILogger<InMemoryStripeWebhookQueue> _logger;
        private readonly IServiceProvider _services;
        private readonly ConcurrentQueue<(Event StripeEvent, string RawJson)> _fallbackQueue;
        private readonly Channel<(Event StripeEvent, string RawJson)> _channel;

        public InMemoryStripeWebhookQueue(
            ILogger<InMemoryStripeWebhookQueue> logger,
            IServiceProvider services)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _fallbackQueue = new ConcurrentQueue<(Event, string)>();

            var options = new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = false
            };
            _channel = Channel.CreateUnbounded<(Event, string)>(options);
        }

        public Task EnqueueAsync(Event stripeEvent, string rawJson)
        {
            if (stripeEvent == null)
            {
                _logger.LogWarning("🚫 Attempted to enqueue null Stripe event.");
                return Task.CompletedTask;
            }

            _fallbackQueue.Enqueue((stripeEvent, rawJson));
            _channel.Writer.TryWrite((stripeEvent, rawJson));

            var queueCount = _fallbackQueue.Count;
            if (queueCount > 50)
            {
                _logger.LogWarning("📛 Stripe webhook queue backlog: {Count}", queueCount);
            }

            _logger.LogInformation("📥 Enqueued Stripe event: {Type} ({Id})", stripeEvent.Type, stripeEvent.Id);
            return Task.CompletedTask;
        }

        public bool TryDequeue(out (Event StripeEvent, string RawJson) item)
        {
            return _fallbackQueue.TryDequeue(out item);
        }

        public async IAsyncEnumerable<(Event StripeEvent, string RawJson)> ReadEventsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            while (await _channel.Reader.WaitToReadAsync(cancellationToken))
            {
                while (_channel.Reader.TryRead(out var item))
                {
                    yield return item;
                }
            }
        }

        public StripeQueueHealthDto GetHealth()
        {
            return new StripeQueueHealthDto
            {
                FallbackQueueCount = _fallbackQueue.Count,
                ChannelReady = !_channel.Reader.Completion.IsCompleted
                // IsHealthy is computed automatically in the DTO
            };
        }
    }
}