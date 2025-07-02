using Stripe;

namespace PropertyManagementAPI.Application.Services.Payments.Stripe
{
    public class StripeService : IStripeService
    {
        private readonly string _secretKey;
        private readonly string _publishableKey;

        public StripeService(string secretKey, string publishableKey)
        {
            _secretKey = secretKey;
            _publishableKey = publishableKey;

            StripeConfiguration.ApiKey = _secretKey;
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
