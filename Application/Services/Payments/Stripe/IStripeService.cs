using Stripe;

namespace PropertyManagementAPI.Application.Services.Payments.Stripe
{
    public interface IStripeService
    {
        Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency);
    }
}
