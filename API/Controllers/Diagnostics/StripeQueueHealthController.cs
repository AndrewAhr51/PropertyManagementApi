using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Infrastructure.Webhooks;
using PropertyManagementAPI.Domain.DTOs.Stripe;

namespace PropertyManagementAPI.API.Controllers.Diagnostics
{
    [ApiController]
    [Route("api/webhooks/stripe/health")]
    [Tags("Diagnostics")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Produces("application/json")]
    public class StripeQueueHealthController : ControllerBase
    {
        private readonly IStripeWebhookQueue _queue;

        public StripeQueueHealthController(IStripeWebhookQueue queue)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
        }

        [HttpGet]
        public ActionResult<StripeQueueHealthDto> GetHealth()
        {
            var health = _queue.GetHealth();
            return health.IsHealthy ? Ok(health) : StatusCode(503, health);
        }
    }
}