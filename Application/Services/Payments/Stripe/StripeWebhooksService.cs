using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PropertyManagementAPI.Application.Services.Payments.Stripe;
using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Repositories.Payments;
using Stripe;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public class StripeWebhookService : IStripeWebhookService
{
    private readonly ILogger<StripeWebhookService> _logger;
    private readonly IStripeRepository _stripeRepository;
    private readonly IPaymentRepository _PaymentRepository;
    private readonly MySqlDbContext _context;

    public StripeWebhookService(
        ILogger<StripeWebhookService> logger,
        IStripeRepository stripeRepository,
        MySqlDbContext context,
        IPaymentRepository paymentRepository)
    {
        _logger = logger;
        _stripeRepository = stripeRepository;
        _context = context;
        _PaymentRepository = paymentRepository;
    }

    public async Task HandleEventAsync(Event stripeEvent, string rawJson)
    {
        _logger.LogInformation("🎯 Enqueuing event: {Type}, ID: {Id}", stripeEvent.Type, stripeEvent.Id);
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
            case StripeEvents.CheckoutSessionCompleted:
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;

                if (session == null)
                {
                    _logger.LogWarning("⚠️ Could not parse Checkout.Session from webhook.");
                    break;
                }

                _logger.LogInformation("💳 Checkout complete for session {SessionId}", session.Id);

                var invoiceIdStr = session.Metadata?.GetValueOrDefault("invoiceId");
                var tenantIdStr = session.ClientReferenceId;

                if (!string.IsNullOrEmpty(invoiceIdStr) && int.TryParse(invoiceIdStr, out int invoiceId) && int.TryParse(tenantIdStr, out int tenantId))
                {
                    var amount = session.AmountTotal.HasValue ? (decimal)(session.AmountTotal.Value / 100.0m) : 0m;

                    var createPaymentDto = new CreatePaymentDto
                    {
                        Amount = amount,
                        Currency = session.Currency,
                        PaymentMethod = "Card",
                        TenantId = tenantId,
                        InvoiceId = invoiceId
                    };

                    var payment = await _PaymentRepository.ProcessPaymentAsync(createPaymentDto);
                    
                    if (payment == null)
                    {
                        _logger.LogWarning("❌ Failed to process payment for session: {Id}", session.Id);
                        return;
                    }

                    _logger.LogInformation("✅ Payment recorded for invoice: {InvoiceId}", invoiceId);
                }
                else
                {
                    _logger.LogWarning("⚠️ Missing or invalid invoiceId or tenantId in session metadata.");
                }

                break;
            case StripeEvents.PaymentIntentSucceeded:
                var succeededIntent = stripeEvent.Data.Object as PaymentIntent;
                if (succeededIntent != null)
                {
                    _logger.LogInformation("✅ Payment succeeded: {Id}", succeededIntent.Id);
                }

                break;
            case StripeEvents.PaymentIntentFailed:
                var failedIntent = stripeEvent.Data.Object as PaymentIntent;
                if (failedIntent != null)
                {
                    _logger.LogWarning("❌ Payment failed: {Id} | {Reason}", failedIntent.Id, failedIntent.LastPaymentError?.Message);
                }
                break;

            default:
                _logger.LogDebug("📦 Skipping unhandled event type: {Type}", stripeEvent.Type);
                break;
        }
    }
}