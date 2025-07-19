using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Infrastructure.Webhooks;

namespace PropertyManagementAPI.API.Controllers.Diagnostics
{
    [ApiController]
    [Route("api/webhooks/diagnostics")]
    [Tags("Stripe-Diagnostics")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Produces("application/json")]
    public class WebhookDiagnosticsController : ControllerBase
    {
        private readonly IWebhookDiagnosticsStore _diagnostics;

        public WebhookDiagnosticsController(IWebhookDiagnosticsStore diagnostics)
        {
            _diagnostics = diagnostics;
        }

        [HttpGet("{sessionId}")]
        public IActionResult GetBySessionId(string sessionId)
        {
            var records = _diagnostics.GetBySessionId(sessionId);
            return Ok(records);
        }

        [HttpGet("events/{eventId}")]
        public IActionResult GetByEventId(string eventId)
        {
            var record = _diagnostics.GetByEventId(eventId);
            return record != null ? Ok(record) : NotFound();
        }

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var records = _diagnostics.GetAll();
            return Ok(records);
        }
    }

}
