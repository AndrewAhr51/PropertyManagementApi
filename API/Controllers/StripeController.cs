using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Payments.Stripe;
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.DTOs.Payments.Stripe;
using PropertyManagementAPI.Domain.DTOs.Stripe;
using PropertyManagementAPI.Infrastructure.Payments;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Payments;
using Stripe;
using Stripe.Checkout;
using System.Threading.Tasks;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/payments/stripe")]
    [Tags("Stripe-Payments")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Produces("application/json")]
    public class PaymentsStripeController : ControllerBase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<PaymentsStripeController> _logger;
        private readonly IPaymentRepository _paymentRepository;
        private readonly PaymentAuditLogger _auditLogger;
        private readonly IStripeService _stripeService;
        private readonly StripeOptions _stripeOptions;
        private readonly IConfiguration _config; // ✅ Added config

        public PaymentsStripeController(
            IInvoiceRepository invoiceRepository,
            ILogger<PaymentsStripeController> logger,
            IPaymentRepository paymentRepository,
            PaymentAuditLogger auditLogger,
            IStripeService stripeService,
            StripeOptions stripeOptions,
            IConfiguration config)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _paymentRepository = paymentRepository;
            _auditLogger = auditLogger;
            _stripeService = stripeService;
            _stripeOptions = stripeOptions;
            _config = config;
        }

        [HttpGet("ping")]
        public IActionResult Ping() => Ok("Stripe controller is alive");

        [HttpPost("stripe-payment")]
        public async Task<IActionResult> ProcessStripePaymentAsync([FromBody] CreateStripeDto dto)
        {
            try
            {
                var response = await _stripeService.ProcessStripePaymentAsync(dto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stripe payment failed for InvoiceId {InvoiceId}", dto.InvoiceId);
                return StatusCode(500, new { message = "Stripe payment failed.", ex.Message });
            }
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateStripeDto dto)
        {
            _logger.LogInformation("🔄 Incoming Stripe DTO: {@Dto}", dto);

            try
            {
                if (dto.Amount <= 0)
                {
                    _logger.LogWarning("❌ Invalid amount provided: {Amount}", dto.Amount);
                    return BadRequest("Amount must be greater than zero.");
                }

                var baseUrl = _config["AppSettings:PublicUrl"];
                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    _logger.LogError("❌ PublicUrl not configured.");
                    return StatusCode(500, "Missing public redirect URL");
                }

                var invoice = await _invoiceRepository.GetInvoiceByIdAsync(dto.InvoiceId);
                if (invoice == null)
                {
                    _logger.LogWarning("⚠️ Invoice not found for ID: {InvoiceId}", dto.InvoiceId);
                    return NotFound(new { error = "Invoice not found." });
                }

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    Mode = "payment",
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "usd",
                                UnitAmount = (long)(dto.Amount * 100), // Stripe expects cents
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = $"Invoice #{dto.InvoiceId}",
                                    Description = $"Tenant: {dto.TenantId}, Property: {dto.PropertyId}"
                                }
                            },
                            Quantity = 1
                        }
                    },
                    SuccessUrl = $"{baseUrl}/payment-success?session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{baseUrl}/payment-cancel",
                    Metadata = new Dictionary<string, string>
                    {
                        { "invoiceId", invoice.InvoiceId.ToString() },
                        { "tenantId", invoice.TenantId.ToString() },
                        { "tenantName", invoice.TenantName ?? string.Empty },
                        { "propertyId", invoice.PropertyId.ToString() },
                        { "propertyName", invoice.PropertyName ?? string.Empty },
                        { "ownerId", invoice.OwnerId.ToString() } // optional
                    }
                };

                var service = new SessionService();
                var session = service.Create(options);

                return Ok(new CheckoutUrlDto { checkoutUrl = session.Url });
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "💥 Stripe API error during checkout session creation.");
                return StatusCode(500, $"Stripe error: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Unexpected error during Stripe checkout session.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}