using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using PropertyManagementAPI.Application.Services.Payments;
using PropertyManagementAPI.Application.Services.Payments.Stripe;
using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.DTOs.Payments.Stripe;
using PropertyManagementAPI.Domain.Entities.Payments.Banking;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Payments")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Produces("application/json")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentsController> _logger;
        private readonly IStripeService _stripeService;

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger, IStripeService stripeService)
        {
            _paymentService = paymentService;
            _logger = logger;
            _stripeService = stripeService;

            _logger.LogInformation("✅ PaymentsController constructor hit");

        }

        // POST: api/payments
        [HttpPost("process-payment")]
        public async Task<ActionResult<PaymentResponseDto>> ProcessPayment([FromBody] CreatePaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var payment = await _paymentService.ProcessPaymentAsync(dto);

                var response = new PaymentResponseDto
                {
                    PaymentId = payment.PaymentId,
                    Amount = payment.Amount,
                    PaidOn = payment.PaidOn,
                    ReferenceNumber = payment.ReferenceNumber,
                    PaymentMethod = dto.PaymentMethod,
                    InvoiceId = payment.InvoiceId,
                    TenantId = payment.TenantId ?? 0, // Explicit cast with null-coalescing operator
                    OwnerId = payment.OwnerId ?? 0   // Explicit cast with null-coalescing operator
                };

                return CreatedAtAction(nameof(GetPaymentById), new { paymentId = payment.PaymentId }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/payments
        [HttpPost("reverse-payment/{paymentId}")]
        public async Task<ActionResult<bool>> ReversePayment(int paymentId)
        {
            try
            {
                var save = await _paymentService.ReversePaymentAsync(paymentId);

                return save ? Ok(new { message = "Payment reversed successfully." }) : BadRequest(new { error = "Failed to reverse payment." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/payments/{paymentId}
        [HttpGet("{paymentId}")]
        public async Task<ActionResult<PaymentResponseDto>> GetPaymentById(int paymentId)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(paymentId);
            if (payment == null)
                return NotFound();

            var response = new PaymentResponseDto
            {
                PaymentId = payment.PaymentId,
                Amount = payment.Amount,
                PaidOn = payment.PaidOn,
                ReferenceNumber = payment.ReferenceNumber,
                PaymentMethod = payment switch
                {
                    CardPayment => "Card",
                    CheckPayment => "Check",
                    ElectronicTransferPayment => "Transfer",
                    _ => "Unknown"
                },
                InvoiceId = payment.InvoiceId,
                TenantId = payment.TenantId ?? 0, // Explicit cast with null-coalescing operator
                OwnerId = payment.OwnerId ?? 0   // Explicit cast with null-coalescing operator
            };

            return Ok(response);
        }

        // GET: api/payments/invoice/{invoiceId}
        [HttpGet("invoice/{invoiceId}")]
        public async Task<ActionResult<IEnumerable<PaymentResponseDto>>> GetPaymentsByInvoice(int invoiceId)
        {
            var payments = await _paymentService.GetPaymentsByInvoiceIdAsync(invoiceId);

            var response = payments.Select(p => new PaymentResponseDto
            {
                PaymentId = p.PaymentId,
                Amount = p.Amount,
                PaidOn = p.PaidOn,
                ReferenceNumber = p.ReferenceNumber,
                PaymentMethod = p switch
                {
                    CardPayment => "Card",
                    CheckPayment => "Check",
                    ElectronicTransferPayment => "Transfer",
                    _ => "Unknown"
                },
                InvoiceId = p.InvoiceId,
                TenantId = p.TenantId ?? 0, // Explicit cast with null-coalescing operator
                OwnerId = p.OwnerId ?? 0   // Explicit cast with null-coalescing operator
            });

            return Ok(response);
        }

        [HttpGet("session/{sessionId}")]
        [ProducesResponseType(typeof(StripeSessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StripeSessionDto>> GetStripeSession(string sessionId)
        {
            _logger.LogInformation("🚀 Entered GetStripeSession with ID: {SessionId}", sessionId);

            if (string.IsNullOrWhiteSpace(sessionId))
                return BadRequest(new ApiErrorDto { Error = "Missing or invalid session ID." });
            try
            {
                var session = await _stripeService.GetSessionAsync(sessionId);

                if (session == null)
                {
                    _logger.LogWarning("⚠️ Stripe session not found: {SessionId}", sessionId);
                    return NotFound(new { error = "Session not found." });
                }

                return Ok(session);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Error retrieving Stripe session: {SessionId}", sessionId);
                return StatusCode(500, new { error = "Internal server error." });
            }
        }

        public class ApiErrorDto
        {
            public string Error { get; set; } = string.Empty;
        }

    }
}
