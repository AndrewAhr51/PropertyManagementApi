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

    public StripeWebhookController(
        ILogger<StripeWebhookController> logger,
        IConfiguration config,
        IStripeWebhookQueue webhookQueue)
    {
        _logger = logger;
        _config = config;
        _webhookQueue = webhookQueue;
    }


    [HttpPost("api/webhooks/stripe")]
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
        try
        {
            stripeEvent = EventUtility.ConstructEvent(json, signature, secret, throwOnApiVersionMismatch: false);
            _logger.LogInformation("✅ Stripe event verified: {Type} ({Id})", stripeEvent.Type, stripeEvent.Id);
        }
        catch (StripeException ex)
        {
            _logger.LogWarning(ex, "❌ Stripe signature verification failed.");
            return BadRequest("Invalid signature");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔥 Error processing Stripe webhook.");
            return StatusCode(500, "Internal server error");
        }

        // Optional: Skip irrelevant event types
        var relevantEvents = new[] {
        Events.CheckoutSessionCompleted,
        Events.InvoicePaid,
        Events.PaymentIntentSucceeded
    };

        if (!relevantEvents.Contains(stripeEvent.Type))
        {
            _logger.LogInformation("⏭️ Ignoring non-critical event: {Type}", stripeEvent.Type);
            return Ok();
        }

        // 📨 Dispatch to background queue
        await _webhookQueue.EnqueueAsync(stripeEvent, json);

        return Ok();
    }
}