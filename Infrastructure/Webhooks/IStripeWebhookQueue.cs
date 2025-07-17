using Stripe;
namespace PropertyManagementAPI.Infrastructure.Webhooks
{
    public interface IStripeWebhookQueue
    {
        Task EnqueueAsync(Event stripeEvent, string rawJson);

    }
}
