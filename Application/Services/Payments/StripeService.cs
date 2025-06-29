using Stripe;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public class StripeService : IStripeService
    {
        public StripeService(IConfiguration config)
        {
            StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
        }

        public async Task<PaymentIntent> CreatePaymentIntentAsync(decimal amount, string currency)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)amount,
                Currency = currency,
                PaymentMethodTypes = new List<string> { "card" }
            };

            var service = new PaymentIntentService();
            return await service.CreateAsync(options);
        }
    }
}
