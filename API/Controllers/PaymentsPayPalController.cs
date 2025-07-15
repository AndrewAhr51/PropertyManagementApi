using Microsoft.AspNetCore.Mvc;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using PropertyManagementAPI.Application.Services.Invoices;
using PropertyManagementAPI.Application.Services.Payments.PayPal;
using PropertyManagementAPI.Domain.DTOs.Payments.PayPal;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;
using PropertyManagementAPI.Infrastructure.Payments;
using System.Diagnostics;
using static System.Net.WebRequestMethods;

namespace PropertyManagementAPI.API.Controllers
{
    [ApiController]
    [Route("api/payments/paypal")]
    [Tags("PayPal-Payments")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Produces("application/json")]
    public class PaymentsPayPalController : ControllerBase
    {
        private readonly IPayPalService _payPalService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILogger<PaymentsPayPalController> _logger;
        private readonly PayPalHttpClient _payPalClient;

        public PaymentsPayPalController(IPayPalService payPalService, IInvoiceService invoiceService, ILogger<PaymentsPayPalController> logger, PayPalClient payPalClient)
        {
            _payPalService = payPalService;
            _invoiceService = invoiceService;
            _logger = logger;
            _payPalClient = payPalClient.Client();
        }

        // 1️⃣ Step 1: Initialize the order (CreatePayPalOrderAsync)
        [HttpPost("initialize")]
        public async Task<ActionResult<PayPalPaymentResponseDto>> InitializePayPal([FromBody] CreatePayPalDto dto)
        {
            var result = await _payPalService.InitializePayPalAsync(dto);

            if (string.IsNullOrWhiteSpace(result?.ApprovalLink))
                return BadRequest(new { error = "Approval link not returned by PayPal." });

            // Return 201 Created with approval link and order metadata
            return CreatedAtAction(
                nameof(CapturePayPalOrder),
                new { orderId = result.OrderId },
                result
            );
        }

        // 2️⃣ Step 2 (Optional): Get approval link from OrderId (if needed separately)
        [HttpGet("approval-link/{orderId}")]
        public async Task<ActionResult<string>> GetApprovalLink(string orderId)
        {
            var approvalUrl = await _payPalService.GetPayPalApprovalLinkAsync(orderId);
            return Ok(approvalUrl);
        }

        // 3️⃣ Step 3: Capture the approved order
        [HttpPost("capture")]
        public async Task<ActionResult<CardPayment>> CaptureOrder([FromBody] CapturePayPalDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.OrderId))
                    return BadRequest("OrderId and request payload must be provided.");

                var invoiceDto = await _invoiceService.GetInvoiceByIdAsync(dto.InvoiceId);
                if (invoiceDto == null)
                    return NotFound($"Invoice {dto.InvoiceId} not found");

                // Populate Invoice entity with values from InvoiceDto
                Invoice invoice = new Invoice
                {
                    InvoiceId = invoiceDto.InvoiceId,
                    PropertyId = invoiceDto.PropertyId,
                    TenantId = invoiceDto.TenantId,
                    OwnerId = invoiceDto.OwnerId,
                    Email = invoiceDto.Email,
                    Status = invoiceDto.Status,
                    CreatedBy = invoiceDto.CreatedBy,
                    Notes = invoiceDto.Notes
                };

                var payment = await _payPalService.CapturePayPalCardPaymentAsync(dto.OrderId, dto.ToCreatePayPalDto(), invoice);

                return Ok(payment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing PayPal payment for OrderId {OrderId}, InvoiceId {InvoiceId}", dto?.OrderId, dto?.InvoiceId);

                // Optional: audit failure (if your controller has access to _auditLogger or via PayPalService)

                return StatusCode(500, $"Unable to capture payment for OrderId {dto?.OrderId}");
            }
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
        [HttpPost("capture/{orderId}")]
        public async Task<IActionResult> CapturePayPalOrder(string orderId)
        {
            await _payPalService.CaptureOrderAsync(orderId);
            return Ok(new { message = "Order captured successfully." });
        }
    }
}
