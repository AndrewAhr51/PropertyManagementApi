using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Payments;
using PropertyManagementAPI.Domain.DTOs.Payments;
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

        public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        // POST: api/payments
        [HttpPost("standard")]
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

    }
}
