using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Stripe;
using System.IO;
using System.Threading.Tasks;
using PropertyManagementAPI.Infrastructure.Webhooks;
using PropertyManagementAPI.Application.Services.Payments.Stripe;

[ApiController]
[Route("api/webhooks/stripe")]
[Tags("Stripe-Webhook")]
[ApiExplorerSettings(GroupName = "v1")]
[Produces("application/json")]
public class StripeWebhookController : ControllerBase
{
    private readonly ILogger<StripeWebhookController> _logger;
    private readonly IConfiguration _config;
    private readonly IStripeWebhookQueue _webhookQueue;
    private readonly IWebhookDiagnosticsStore _diagnosticsStore;


    public StripeWebhookController(
    ILogger<StripeWebhookController> logger,
    IConfiguration config,
    IStripeWebhookQueue webhookQueue,
    IWebhookDiagnosticsStore diagnosticsStore)
    {
        _logger = logger;
        _config = config;
        _webhookQueue = webhookQueue;
        _diagnosticsStore = diagnosticsStore;
    }

    [HttpPost]
    public async Task<IActionResult> HandleWebhook()
    {
        HttpContext.Request.EnableBuffering();

        string json;
        using (var reader = new StreamReader(HttpContext.Request.Body, leaveOpen: true))
        {
            json = await reader.ReadToEndAsync();
            HttpContext.Request.Body.Position = 0;
        }

        var signature = Request.Headers["Stripe-Signature"];
        var secret = _config["Stripe:WebhookSecret"];

        if (string.IsNullOrWhiteSpace(json))
        {
            _logger.LogWarning("⚠️ Stripe webhook received empty payload.");
            return BadRequest("Empty payload");
        }

        if (string.IsNullOrWhiteSpace(signature))
        {
            _logger.LogWarning("⚠️ Missing Stripe-Signature header.");
            return BadRequest("Missing signature");
        }

        Event stripeEvent;
        Stripe.Checkout.Session session = null;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, signature, secret, throwOnApiVersionMismatch: false);

            _logger.LogInformation("[🔔 Webhook Received] Type: {Type} | ID: {Id} | Time: {Time}",
                stripeEvent.Type, stripeEvent.Id, DateTime.UtcNow);

            if (stripeEvent.Type == Events.CheckoutSessionCompleted &&
                stripeEvent.Data.Object is Stripe.Checkout.Session s)
            {
                session = s;

                _logger.LogInformation("[📦 Checkout Session Metadata] ID: {SessionId} | InvoiceID: {InvoiceId} | TenantID: {TenantId} | TenantName: {TenantName} | PropertyName: {PropertyName}",
                    session.Id,
                    session.Metadata?["invoiceId"],
                    session.Metadata?["tenantId"],
                    session.Metadata?["tenantName"],
                    session.Metadata?["propertyName"]
                );

                _diagnosticsStore.Log(new WebhookEventRecord
                {
                    EventId = stripeEvent.Id,
                    SessionId = session.Id,
                    EventType = stripeEvent.Type,
                    Outcome = "Received",
                    ReceivedAt = DateTime.UtcNow,
                    ErrorDetails = ""
                });
            }
            else
            {
                _diagnosticsStore.Log(new WebhookEventRecord
                {
                    EventId = stripeEvent.Id,
                    SessionId = "unknown",
                    EventType = stripeEvent.Type,
                    Outcome = "Received",
                    ReceivedAt = DateTime.UtcNow,
                    ErrorDetails = ""
                });
            }
        }
        catch (StripeException ex)
        {
            _logger.LogWarning(ex, "❌ Stripe signature verification failed.");

            _diagnosticsStore.Log(new WebhookEventRecord
            {
                EventId = Guid.NewGuid().ToString(),
                SessionId = "unknown",
                EventType = "Invalid",
                Outcome = "Error",
                ReceivedAt = DateTime.UtcNow,
                ErrorDetails = ex.Message
            });

            return BadRequest("Invalid signature");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔥 Error processing Stripe webhook.");

            _diagnosticsStore.Log(new WebhookEventRecord
            {
                EventId = Guid.NewGuid().ToString(),
                SessionId = "unknown",
                EventType = "Error",
                Outcome = "Exception",
                ReceivedAt = DateTime.UtcNow,
                ErrorDetails = ex.Message
            });

            return StatusCode(500, "Internal server error");
        }

        var relevantEvents = new[]
        {
        Events.CheckoutSessionCompleted,
        Events.InvoicePaid,
        Events.PaymentIntentSucceeded
    };

        if (!relevantEvents.Contains(stripeEvent.Type))
        {
            _logger.LogInformation("⏭️ Ignoring non-critical event: {Type}", stripeEvent.Type);
            return Ok();
        }

        await _webhookQueue.EnqueueAsync(stripeEvent, json);

        if (stripeEvent.Type == Events.CheckoutSessionCompleted &&
            session != null && !string.IsNullOrEmpty(session.Id))
        {
            var redirectUrl = GetRedirectUrl(session.Id);
            return Ok(new { redirectUrl });
        }

        return Ok();
    }

    private string GetRedirectUrl(string sessionId)
    {
        var env = _config["ASPNETCORE_ENVIRONMENT"] ?? "Development";
        var baseUrl = env switch
        {
            "Production" => "https://app.omnitenant.com",
            "Staging" => "https://staging.omnitenant.com",
            _ => "http://localhost:4200"
        };

        return $"{baseUrl}/payment-success?session_id={sessionId}";
    }
}