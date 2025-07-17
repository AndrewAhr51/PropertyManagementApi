using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Stripe;
using System.IO;
using System.Threading.Tasks;
using PropertyManagementAPI.Infrastructure.Webhooks;

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

    [ApiController]
    [Route("api/webhook-test")]
    public class StripeWebhookTestController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            //var secret = _config["Stripe:WebhookSecret"];
            var secret = "whsec_09568ce978fc7dde1d1ab44e327e4b70f491b09c7a990e4b365082bcdfdf4f6e5";
            var signature = Request.Headers["Stripe-Signature"];

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, signature, secret);
                return Ok("✅ Event verified: " + stripeEvent.Type);
            }
            catch (Exception ex)
            {
                return BadRequest("❌ Verification failed: " + ex.Message);
            }
        }
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