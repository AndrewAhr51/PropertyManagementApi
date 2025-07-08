using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using System.Threading.Tasks;

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
            _logger = logger;
            _services = services;
        }

        public async Task EnqueueAsync(Event stripeEvent, string rawJson)
        {
            using var scope = _services.CreateScope();
            var webhookService = scope.ServiceProvider.GetRequiredService<IStripeWebhookService>();

            try
            {
                await webhookService.HandleEventAsync(stripeEvent, rawJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to handle Stripe event {Id}", stripeEvent.Id);
            }
        }
    }
}