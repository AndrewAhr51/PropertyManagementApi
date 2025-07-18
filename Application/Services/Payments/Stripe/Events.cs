namespace PropertyManagementAPI.Application.Services.Payments.Stripe
{
    public static class Events
    {
        public const string CheckoutSessionCompleted = "checkout.session.completed";
        public const string InvoicePaid = "invoice.paid";
        public const string PaymentIntentSucceeded = "payment_intent.succeeded";
        // Add more as needed
    }
}
