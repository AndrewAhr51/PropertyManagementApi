using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Payments;
using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.Entities.Payments;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // POST: api/payments
        [HttpPost]
        public async Task<ActionResult<PaymentResponseDto>> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var payment = await _paymentService.CreatePaymentAsync(dto);

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
