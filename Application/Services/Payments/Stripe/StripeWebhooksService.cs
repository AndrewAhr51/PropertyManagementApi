using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Application.Services.Payments.Stripe;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Repositories.Payments;
using Stripe;
using System.Threading.Tasks;

public class StripeWebhookService : IStripeWebhookService
{
    private readonly ILogger<StripeWebhookService> _logger;
    private readonly IStripeRepository _stripeRepository;
    private readonly MySqlDbContext _context;

    public StripeWebhookService(
        ILogger<StripeWebhookService> logger,
        IStripeRepository stripeRepository,
        MySqlDbContext context)
    {
        _logger = logger;
        _stripeRepository = stripeRepository;
        _context = context;
    }

    public async Task HandleEventAsync(Event stripeEvent, string rawJson)
    {
        // 💾 Optional: persist for audit trail
        if (!_context.StripeWebhookEvents.Any(e => e.StripeEventId == stripeEvent.Id))
        {
            _context.StripeWebhookEvents.Add(new StripeWebhookEvent
            {
                StripeEventId = stripeEvent.Id,
                EventType = stripeEvent.Type,
                Payload = rawJson
            });

            await _context.SaveChangesAsync();
        }

        switch (stripeEvent.Type)
        {
            case "payment_intent.succeeded":
                var intent = stripeEvent.Data.Object as PaymentIntent;
                _logger.LogInformation("✅ Payment confirmed: {Id}", intent.Id);

                // Insert domain logic or queue finalization
                break;

            case "payment_intent.payment_failed":
                var failedIntent = stripeEvent.Data.Object as PaymentIntent;
                _logger.LogWarning("❌ Payment failed: {Id} | {Reason}", failedIntent.Id, failedIntent?.LastPaymentError?.Message);
                break;

            default:
                _logger.LogDebug("📦 Skipping unhandled event type: {Type}", stripeEvent.Type);
                break;
        }
    }
}