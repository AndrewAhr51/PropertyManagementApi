using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Payments;
using PropertyManagementAPI.Domain.DTOs.Payments;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // POST: api/payment
        [HttpPost]
        public async Task<ActionResult<PaymentDto>> CreatePayment([FromBody] CreatePaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var payment = await _paymentService.CreatePaymentAsync(dto);
            return CreatedAtAction(nameof(GetPayment), new { paymentId = payment.PaymentId }, payment);
        }

        // GET: api/payment/{paymentId}
        [HttpGet("{paymentId}")]
        public async Task<ActionResult<PaymentDto>> GetPayment(int paymentId)
        {
            var payment = await _paymentService.GetPaymentAsync(paymentId);
            if (payment is null)
                return NotFound();

            return Ok(payment);
        }

        // GET: api/payment/tenant/{tenantId}
        [HttpGet("tenant/{tenantId}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPaymentsByTenant(int tenantId)
        {
            var payments = await _paymentService.GetPaymentsByTenantAsync(tenantId);
            return Ok(payments);
        }
    }
}
