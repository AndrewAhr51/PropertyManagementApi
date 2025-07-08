
using Stripe;

namespace PropertyManagementAPI.Application.Services.Payments.Stripe
{
    public interface IStripeWebhookService
    {   
        Task HandleEventAsync(Event stripeEvent, string rawJson);
    }

}
