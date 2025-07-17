using Microsoft.Extensions.Logging;
using Stripe;
using System;
using System.Threading.Tasks;

namespace PropertyManagementAPI.Infrastructure.Webhooks
{
    public class StripeWebhookQueue : IStripeWebhookQueue
    {
        private readonly ILogger<StripeWebhookQueue> _logger;

        public StripeWebhookQueue(ILogger<StripeWebhookQueue> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task EnqueueAsync(Event stripeEvent, string rawJson)
        {
            _logger.LogInformation("📨 EnqueueAsync called with event: {Id}", stripeEvent?.Id);
            try
            {
                if (stripeEvent == null)
                {
                    _logger.LogWarning("🚫 Null Stripe event passed to queue.");
                    return;
                }

                _logger.LogInformation(
                    "📬 Webhook received: {Type} for ID {Id}",
                    stripeEvent.Type,
                    stripeEvent.Id
                );

                // TODO: dispatch to background processor, persist, etc.
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Exception while enqueuing Stripe webhook event {Id}", stripeEvent?.Id);
                throw; // Optional: rethrow or suppress depending on context
            }
        }
    }
}