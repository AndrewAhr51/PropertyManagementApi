using Stripe;
namespace PropertyManagementAPI.Application.Services.Payments.Stripe
{
    public interface IStripeWebhookQueue
    {
        Task EnqueueAsync(Event stripeEvent, string rawJson);
    }

}
