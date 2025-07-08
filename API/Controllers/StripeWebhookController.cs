using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using PropertyManagementAPI.Application.Services.Payments.Stripe;
using Stripe;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/webhooks/stripe")]
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

    [HttpPost]
    public async Task<IActionResult> HandleWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var secret = _config["Stripe:WebhookSecret"];
        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                secret
            );
        }
        catch (StripeException ex)
        {
            _logger.LogWarning(ex, "⚠️ Stripe signature verification failed.");
            return BadRequest();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔥 Unexpected error during Stripe webhook processing.");
            return StatusCode(500);
        }

        // 🚀 Enqueue for async processing
        await _webhookQueue.EnqueueAsync(stripeEvent, json);
        return Ok();
    }
}