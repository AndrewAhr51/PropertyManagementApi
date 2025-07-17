using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using System.Threading.Tasks;
using PropertyManagementAPI.Infrastructure.Webhooks;

namespace PropertyManagementAPI.Application.Services.Payments.Stripe
{
    public class InMemoryStripeWebhookQueue : IStripeWebhookQueue
    {
        private readonly ILogger<InMemoryStripeWebhookQueue> _logger;
        private readonly IServiceProvider _services;

        public InMemoryStripeWebhookQueue(
            ILogger<InMemoryStripeWebhookQueue> logger,
            IServiceProvider services)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public async Task EnqueueAsync(Event stripeEvent, string rawJson)
        {
            if (stripeEvent == null)
            {
                _logger.LogWarning("🚫 Attempted to enqueue null Stripe event.");
                return;
            }

            try
            {
                using var scope = _services.CreateScope();
                var webhookService = scope.ServiceProvider.GetService<IStripeWebhookService>();

                if (webhookService == null)
                {
                    _logger.LogError("🚨 IStripeWebhookService not resolved from scoped provider.");
                    return;
                }

                await webhookService.HandleEventAsync(stripeEvent, rawJson);
                _logger.LogInformation("✅ Stripe event handled: {Type} (ID: {Id})", stripeEvent.Type, stripeEvent.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to handle Stripe event {Id}", stripeEvent.Id);
            }
        }
    }
}