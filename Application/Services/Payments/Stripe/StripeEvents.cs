namespace PropertyManagementAPI.Application.Services.Payments.Stripe
{
    public static class StripeEvents
    {
        public const string CheckoutSessionCompleted = "checkout.session.completed";
        public const string PaymentIntentSucceeded = "payment_intent.succeeded";
        public const string PaymentIntentFailed = "payment_intent.payment_failed";

    }
}
