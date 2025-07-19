using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using PropertyManagementAPI.Common.Helpers;
using System.Text.Json;
using System.Text.Json.Nodes;
using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Application.Services.Invoices;
using PropertyManagementAPI.Application.Services.Payments;


namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/webhooks/paypal")]
    [Tags("PayPal-Webhook")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Produces("application/json")]
    public class PayPalWebhookController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IInvoiceService _invoiceService;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PayPalWebhookController> _logger;
        private readonly PaymentAuditLogger _auditLogger;

        public PayPalWebhookController(IConfiguration config, IInvoiceService invoiceService, IPaymentService paymentService,
                                        ILogger<PayPalWebhookController> logger, PaymentAuditLogger auditLogger)
        {
            _config = config;
            _invoiceService = invoiceService;
            _paymentService = paymentService;
            _logger = logger;
            _auditLogger = auditLogger;
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

            _logger.LogInformation("🔔 PayPal Webhook Received: {Payload}", json);

            try
            {
                var root = JsonNode.Parse(json);
                var eventType = root?["event_type"]?.ToString();
                var invoiceIdRaw = root?["resource"]?["custom_id"]?.ToString();

                if (string.IsNullOrWhiteSpace(eventType))
                    return BadRequest("Missing event_type");

                switch (eventType)
                {
                    case "PAYMENT.CAPTURE.COMPLETED":
                        if (int.TryParse(invoiceIdRaw, out int invoiceId))
                        {
                            var invoice = await _invoiceService.GetInvoiceByIdAsync(invoiceId);
                            if (invoice == null)
                            {
                                _logger.LogWarning("⚠️ Invoice {InvoiceId} not found.", invoiceId);
                                break;
                            }

                            CreatePaymentDto createPaymentDto = new CreatePaymentDto
                            {
                                InvoiceId = invoice.InvoiceId,
                                Amount = invoice.Amount,
                                TenantId = invoice.TenantId,
                                OwnerId = invoice.OwnerId,
                                PaidOn = DateTime.UtcNow,
                                PaymentMethod = "PayPal",
                            };
                            invoice.IsPaid = true;
                            invoice.Status = "Paid";
                            await _paymentService.ProcessPaymentAsync(createPaymentDto);

                            await _auditLogger.LogAsync("PayPal", eventType, invoiceId);
                            _logger.LogInformation("✅ Invoice {InvoiceId} marked as paid.", invoiceId);
                        }
                        else
                        {
                            _logger.LogWarning("⚠️ InvoiceId parsing failed: {Raw}", invoiceIdRaw);
                        }
                        break;

                    case "PAYMENT.CAPTURE.DENIED":
                        _logger.LogWarning("❌ Payment denied for invoice: {InvoiceId}", invoiceIdRaw);
                        await _auditLogger.LogAsync("PayPal", eventType, invoiceIdRaw);
                        break;

                    default:
                        _logger.LogInformation("ℹ️ Unhandled PayPal event type: {EventType}", eventType);
                        break;
                }

                return Ok(); // ✅ Confirm receipt to PayPal
            }
            catch (JsonException je)
            {
                _logger.LogError(je, "❌ Failed to parse webhook payload.");
                return BadRequest("Invalid JSON payload");
            }


        }
    }
}
