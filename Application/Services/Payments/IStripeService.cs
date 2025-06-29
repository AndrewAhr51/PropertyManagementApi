using Stripe;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public interface IStripeService
    {
        Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency);
    }
}
