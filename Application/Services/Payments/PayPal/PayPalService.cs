using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.DTOs.Payments.PayPal;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Payments;

namespace PropertyManagementAPI.Application.Services.Payments.PayPal
{
    public class PayPalService : IPayPalService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<PayPalService> _logger;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPayPalPaymentProcessor _payPalPaymentProcessor;
        private readonly PaymentAuditLogger _auditLogger;
        private readonly IPayPalRepository _payPalRepository;
        private readonly PayPalHttpClient _payPalHttpClient;

        public PayPalService(IInvoiceRepository invoiceRepository, ILogger<PayPalService> logger, IPaymentRepository paymentRepository,
                            IPayPalPaymentProcessor payPalPaymentProcessor, PaymentAuditLogger auditLogger, PayPalHttpClient payPalHttpClient)
        {
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _paymentRepository = paymentRepository;
            _payPalPaymentProcessor = payPalPaymentProcessor;
            _auditLogger = auditLogger;
            _payPalHttpClient = payPalHttpClient;
        }

        //Step 1: Initialize PayPal Order
        public async Task<PayPalPaymentResponseDto> InitializePayPalAsync(CreatePayPalDto dto)
        {
            Invoice invoice = new Invoice(); // Initialize to avoid null reference in case of exception
            try
            {
                if (dto == null)
                    throw new ArgumentNullException(nameof(dto), "InitializePayPalAsync cannot be null");
                if (dto.InvoiceId <= 0)
                    throw new ArgumentException("Invalid InvoiceId", nameof(dto.InvoiceId));

                invoice = await _invoiceRepository.GetInvoiceByIdAsync(dto.InvoiceId) ?? throw new ArgumentException($"Invoice {dto.InvoiceId} not found");

                if (invoice == null)
                    throw new ArgumentException($"Invoice {dto.InvoiceId} not found");

                var idempotencyKey = HashHelper.Sha256($"paypal:{invoice.TenantId}:{dto.InvoiceId}");

                var orderResult = await _payPalPaymentProcessor.InitializePayPalOrderAsync(invoice.Amount, "USD", invoice, idempotencyKey);

                var responseDto = new PayPalPaymentResponseDto
                {
                    OrderId = orderResult.OrderId,
                    ApprovalLink = orderResult.ApprovalLink,
                    Status = "CREATED",
                    Amount = invoice.Amount,
                    CurrencyCode = "USD",
                    InvoiceId = invoice.InvoiceId,
                    InvoiceReference = invoice.ReferenceNumber
                };

                await _auditLogger.LogAsync(
                    action: "InitializePayPalOrder",
                    status: "SUCCESS",
                    response: new
                    {
                        InvoiceId = invoice.InvoiceId,
                        TenantId = invoice.TenantId,
                        OrderId = orderResult.OrderId,
                        ApprovalLink = orderResult.ApprovalLink,
                        Amount = invoice.Amount,
                        CurrencyCode = dto.CurrencyCode,
                        IdempotencyKey = idempotencyKey
                    },
                    performedBy: "Web"
                );

                return responseDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PayPal order for InvoiceId {InvoiceId}", dto?.InvoiceId);

                await _auditLogger.LogAsync(
                    action: "InitializePayPalOrder",
                    status: "FAILED",
                    response: new
                    {
                        InvoiceId = invoice.InvoiceId,
                        TenantId = invoice.TenantId,
                        Amount = invoice?.Amount,
                        Error = ex.Message
                    },
                    performedBy: "Web"
                );

                throw;
            }
        }

        // Step 2: Capture PayPal Card Payment

        public async Task<CardPayment> CapturePayPalCardPaymentAsync(string orderId, CreatePayPalDto dto, Invoice invoice)
        {
            try
            {
                return await _payPalPaymentProcessor.CapturePayPalCardPaymentAsync(orderId, dto, invoice);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing PayPal order {OrderId}", orderId);
                throw;
            }
        }

        public async Task<PayPalOrderResult> CreatePayPalOrderAsync(decimal amount, string currency, Invoice invoice, string idempotencyKey)
        {
            try
            {
                return await _payPalPaymentProcessor.InitializePayPalOrderAsync(amount, currency, invoice, idempotencyKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PayPal order for InvoiceId {InvoiceId}", invoice.InvoiceId);
                throw;
            }
        }

        public async Task<string> GetPayPalApprovalLinkAsync(string orderId)
        {
            try
            {
                return await _payPalPaymentProcessor.GetPayPalApprovalLinkAsync(orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving approval link for OrderId {OrderId}", orderId);
                throw;
            }
        }

        public async Task<PayPalCaptureResponseDto> CaptureOrderAsync(string orderId)
        {
            var request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest()); // required even if empty

            var response = await _payPalHttpClient.Execute(request);
            var order = response.Result<Order>();

            var capture = order.PurchaseUnits
                .SelectMany(pu => pu.Payments?.Captures ?? new List<Capture>())
                .FirstOrDefault();

            return new PayPalCaptureResponseDto
            {
                OrderId = order.Id,
                Status = order.Status,
                CaptureId = capture?.Id,
                Amount = decimal.TryParse(capture?.Amount?.Value, out var val) ? val : 0,
                CurrencyCode = capture?.Amount?.CurrencyCode ?? "USD",
                CaptureTime = DateTime.UtcNow,
                PayerEmail =  "",
                PayerName = $"{order.Payer?.Name?.GivenName} {order.Payer?.Name?.Surname}".Trim()
            };
        }
    }
}
