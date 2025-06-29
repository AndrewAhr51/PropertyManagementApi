using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using PropertyManagementAPI.Application.Services.Payments;
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.Entities.Payments;
using System.Diagnostics;
using static System.Net.WebRequestMethods;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly PayPalHttpClient _payPalClient;
        private readonly PaymentAuditLogger _auditLogger;


        public PaymentsController(IPaymentService paymentService, PayPalClient payPalClient, PaymentAuditLogger auditLogger)
        {
            _paymentService = paymentService;
            _payPalClient = payPalClient.Client();
            _auditLogger = auditLogger;
        }

        // POST: api/payments
        [HttpPost("standard")]
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

        [HttpPost("paypal/order")]
        public async Task<ActionResult<PayPalPaymentResponseDto>> CreatePayPalOrder([FromBody] CreatePayPalDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _paymentService.CreatePayPalOrderAsync(dto);
                return Ok(result);
            }
            catch (HttpException httpEx)
            {
                httpEx.Headers.TryGetValues("Paypal-Debug-Id", out var debugIds);
                var debugId = debugIds?.FirstOrDefault();

                await _auditLogger.LogAsync(
                    action: "GetApprovalLink",
                    status: "FAILED",
                    response: new { Error = httpEx.Message, DebugId = debugId },
                    performedBy: "Web"
                );

                return StatusCode(422, new { error = httpEx.Message });
            }
            catch (Exception ex)
            {
                await _auditLogger.LogAsync(
                   action: "GetApprovalLink",
                   status: "FAILED",
                   response: new { Error = ex.Message },
                   performedBy: "Web"
               );
                return StatusCode(500, new { error = "An unexpected error occurred." });
            }
        }

        [HttpPost("paypal/capture/{orderId}")]
        public async Task<ActionResult<CardPayment>> CapturePayPalOrder(string orderId, [FromBody] CreatePayPalDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var invoice = await _paymentService.GetInvoiceAsync(dto.InvoiceId);
                if (invoice == null)
                    return NotFound(new { error = "Invoice not found." });

                var payment = await _paymentService.CapturePayPalCardPaymentAsync(orderId, dto, invoice);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                await _auditLogger.LogAsync(
                   action: "CapturePayPalOrder",
                   status: "FAILED",
                   response: new { OrderId = orderId, Error = ex.Message },
                   performedBy: "Web"
               );
                return StatusCode(500, new { error = ex.Message });
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

        [HttpPost("testcreate")]
        public async Task<IActionResult> CreateOrder()
        {
            var order = new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>
            {
                new PurchaseUnitRequest
                {
                    AmountWithBreakdown = new AmountWithBreakdown
                    {
                        CurrencyCode = "USD",
                        Value = "100.00"
                    }
                }
            }
            };

            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(order);

            var response = await _payPalClient.Execute(request);
            var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

            return Ok(new { id = result.Id });
        }

        [HttpPost("testcapture/{orderId}")]
        public async Task<IActionResult> CaptureOrder(string orderId)
        {
            var request = new OrdersCaptureRequest(orderId);
            var response = await _payPalClient.Execute(request);
            var result = response.Result<PayPalCheckoutSdk.Orders.Order>();

            return Ok(new { id = result.Id, status = result.Status });
        }

    }

}
