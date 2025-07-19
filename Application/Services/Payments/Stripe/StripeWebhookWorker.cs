using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using System.Threading;
using System.Threading.Tasks;
using PropertyManagementAPI.Infrastructure.Webhooks;
using PropertyManagementAPI.Application.Services.Payments.Stripe;

namespace PropertyManagementAPI.Application.Services.Payments.Stripe
{
    public class StripeWebhookWorker : BackgroundService
    {
        private readonly ILogger<StripeWebhookWorker> _logger;
        private readonly IServiceProvider _services;

        public StripeWebhookWorker(
            ILogger<StripeWebhookWorker> logger,
            IServiceProvider services)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 StripeWebhookWorker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var webhookQueue = scope.ServiceProvider.GetService<IStripeWebhookQueue>();
                var webhookService = scope.ServiceProvider.GetService<IStripeWebhookService>();

                if (webhookQueue == null || webhookService == null)
                {
                    _logger.LogError("🚨 Failed to resolve IStripeWebhookQueue or IStripeWebhookService from scope.");
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                if (webhookQueue.TryDequeue(out var item))
                {
                    try
                    {
                        await webhookService.HandleEventAsync(item.StripeEvent, item.RawJson);
                        _logger.LogInformation("✅ Processed Stripe event: {Type} ({Id})", item.StripeEvent.Type, item.StripeEvent.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "💥 Failed to process Stripe event: {Id}", item.StripeEvent.Id);
                        // Optional: implement retry or error queue here
                    }
                }

                await Task.Delay(500, stoppingToken); // Polling interval
            }

            _logger.LogInformation("🛑 StripeWebhookWorker stopping.");
        }
    }
}