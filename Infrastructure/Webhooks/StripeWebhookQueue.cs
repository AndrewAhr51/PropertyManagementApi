using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Domain.DTOs.Stripe;
using Stripe;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace PropertyManagementAPI.Infrastructure.Webhooks
{
    public class StripeWebhookQueue : IStripeWebhookQueue
    {
        private readonly Channel<(Event StripeEvent, string RawJson)> _channel;

        public StripeWebhookQueue()
        {
            var options = new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = false
            };
            _channel = Channel.CreateUnbounded<(Event, string)>(options);
        }

        public async Task EnqueueAsync(Event stripeEvent, string rawJson)
        {
            if (stripeEvent == null || string.IsNullOrWhiteSpace(rawJson))
                return;

            await _channel.Writer.WriteAsync((stripeEvent, rawJson));
        }

        public bool TryDequeue(out (Event StripeEvent, string RawJson) item)
        {
            return _channel.Reader.TryRead(out item);
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
                FallbackQueueCount = -1, // Channel<T> doesn't expose count directly
                ChannelReady = !_channel.Reader.Completion.IsCompleted,
            };
        }
    }
}