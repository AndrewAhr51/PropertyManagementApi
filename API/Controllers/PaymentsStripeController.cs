using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Payments.Stripe;
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.DTOs.Invoices;
using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.DTOs.Payments.Stripe;
using PropertyManagementAPI.Domain.DTOs.Stripe;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Payments;
using Stripe;
using Stripe.Checkout;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/payments/stripe")]
    [Tags("Stripe-Payments")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Produces("application/json")]
    public class PaymentsStripeController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<StripeService> _logger;
        private readonly IPaymentRepository _paymentRepository;
        private readonly PaymentAuditLogger _auditLogger;
        private readonly IStripeService _stripeService;
        private readonly StripeOptions _stripeOptions;
        public PaymentsStripeController(IInvoiceRepository invoiceRepository, ILogger<StripeService> logger,
            IPaymentRepository paymentRepository, PaymentAuditLogger auditLogger, IStripeService stripeService, StripeOptions stripeOptions)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _paymentRepository = paymentRepository;
            _auditLogger = auditLogger;
            _stripeService = stripeService;
            _stripeOptions = stripeOptions;
        }
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
        public IActionResult CreateCheckoutSession([FromBody] CreateStripeDto dto)
        {
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
                    UnitAmount = (long)(dto.Amount * 100), // Convert dollars to cents
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = $"Invoice #{dto.InvoiceId}",
                        Description = $"Tenant: {dto.TenantId}, Property: {dto.PropertyId}"
                    }
                },
                Quantity = 1
            }
        },
                SuccessUrl = "https://yourapp.com/payment-success",
                CancelUrl = "https://yourapp.com/payment-cancel",
                Metadata = new Dictionary<string, string>
        {
            { "invoiceId", dto.InvoiceId.ToString() },
            { "tenantId", dto.TenantId.ToString() },
            { "propertyId", dto.PropertyId.ToString() }
        }
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Ok(new { checkoutUrl = session.Url });
        }
                
        [HttpPost("stripe-webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
           
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(
                json, Request.Headers["Stripe-Signature"], _stripeOptions.ClientSecret
            );

            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            var metadata = paymentIntent?.Metadata;

            

            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                if (metadata != null && metadata.TryGetValue("invoiceId", out string invoiceIdStr) && int.TryParse(invoiceIdStr, out int invoiceId))
                {
                    // 🔧 Now you can query your database directly by invoiceId
                    var invoice = await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);
                    if (invoice == null)
                    {
                        _logger.LogError("Invoice with ID {InvoiceId} not found.", invoiceId);
                        return NotFound(new { message = "Invoice not found." });
                    }
                    // Update the invoice status to paid
                    invoice.IsPaid = true;
                    invoice.ModifiedDate = DateTime.UtcNow;
                    await _paymentRepository.ProcessPaymentAsync(new CreatePaymentDto
                    {
                        InvoiceId = invoiceId,
                        Amount = paymentIntent.AmountReceived / 100m, // Convert cents to dollars
                        Currency = paymentIntent.Currency,
                        PaidOn = DateTime.UtcNow,
                        PaymentMethod = "Stripe",
                        Metadata = metadata
                    });
                }
            }

            return Ok();
        }
    }
}
