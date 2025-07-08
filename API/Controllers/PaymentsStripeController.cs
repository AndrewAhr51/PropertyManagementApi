using Microsoft.AspNetCore.Mvc;
using PropertyManagementAPI.Application.Services.Payments.Stripe;
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.DTOs.Payments.Stripe;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Payments;

namespace PropertyManagementAPI.API.Controllers
{
    public class PaymentsStripeController : Controller
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<StripeService> _logger;
        private readonly IPaymentRepository _paymentRepository;
        private readonly PaymentAuditLogger _auditLogger;
        private readonly IStripeService _stripeService;
        public PaymentsStripeController(IInvoiceRepository invoiceRepository, ILogger<StripeService> logger,
            IPaymentRepository paymentRepository, PaymentAuditLogger auditLogger, IStripeService stripeService)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _paymentRepository = paymentRepository;
            _auditLogger = auditLogger;
            _stripeService = stripeService;
        }
        [HttpPost("stripe")]
        public async Task<IActionResult> ProcessStripePaymentIntentAsync([FromBody] CreateStripeDto dto)
        {
            try
            {
                var response = await _stripeService.ProcessStripePaymentIntentAsync(dto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Stripe payment failed for InvoiceId {InvoiceId}", dto.InvoiceId);
                return StatusCode(500, new { message = "Stripe payment failed.", ex.Message });
            }
        }
    }
}
