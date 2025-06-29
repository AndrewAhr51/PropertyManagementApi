using Microsoft.Extensions.Logging;
using PayPalCheckoutSdk.Orders;
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Payments;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<PaymentService> _logger;
        private readonly IPaymentProcessor _paymentProcessor;
        private readonly PaymentAuditLogger _auditLogger;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IInvoiceRepository invoiceRepository,
            ILogger<PaymentService> logger,
            IPaymentProcessor paymentProcessor,
            PaymentAuditLogger auditLogger)
        {
            _paymentRepository = paymentRepository;
            _invoiceRepository = invoiceRepository;
            _paymentProcessor = paymentProcessor;
            _logger = logger;
            _auditLogger = auditLogger;
        }

        public async Task<Payment> CreatePaymentAsync(CreatePaymentDto dto)
        {
            try
            {
                return await _paymentRepository.CreatePaymentAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment for InvoiceId {InvoiceId}", dto.InvoiceId);
                throw;
            }
        }

        public async Task<PayPalPaymentResponseDto> CreatePayPalOrderAsync(CreatePayPalDto dto)
        {
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceByIdAsync(dto.InvoiceId);
                if (invoice == null)
                    throw new ArgumentException($"Invoice {dto.InvoiceId} not found");

                var orderId = await _paymentProcessor.CreatePayPalOrderAsync(dto.Amount, "USD", invoice);
                var approvalLink = await _paymentProcessor.GetApprovalLinkAsync(orderId);

                return new PayPalPaymentResponseDto
                {
                    OrderId = orderId,
                    ApprovalLink = approvalLink,
                    Status = "CREATED",
                    Amount = dto.Amount,
                    CurrencyCode = "USD",
                    InvoiceId = invoice.InvoiceId,
                    InvoiceReference = invoice.ReferenceNumber
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PayPal order for InvoiceId {InvoiceId}", dto.InvoiceId);
                throw;
            }
        }

        public async Task<CardPayment> CapturePayPalCardPaymentAsync(string orderId, CreatePayPalDto dto, Invoice invoice)
        {
            try
            {
                var status = await _paymentProcessor.CapturePayPalOrderAsync(orderId);

                if (status != "COMPLETED")
                    throw new InvalidOperationException($"PayPal capture failed with status: {status}");

                var payment = new CardPayment
                {
                    InvoiceId = invoice.InvoiceId,
                    TenantId = invoice.TenantId,
                    OwnerId = invoice.OwnerId,
                    Status = status,
                    OrderId = orderId,
                    PaidOn = DateTime.UtcNow,
                    CardType = dto.Metadata.GetValueOrDefault("CardType"),
                    Last4Digits = dto.Metadata.GetValueOrDefault("Last4Digits"),
                    AuthorizationCode = orderId
                };

                await _paymentRepository.AddPaymentAsync(payment); 
                await _paymentRepository.SavePaymentChangesAsync();
                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing PayPal order {OrderId}", orderId);
                throw;
            }
        }

        public async Task<Invoice?> GetInvoiceAsync(int invoiceId)
        {
            try
            {
                return await _invoiceRepository.GetInvoiceByIdAsync(invoiceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice {InvoiceId}", invoiceId);
                throw;
            }
        }

        public async Task<Payment> GetPaymentByIdAsync(int paymentId)
        {
            try
            {
                return await _paymentRepository.GetPaymentByIdAsync(paymentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment with ID {PaymentId}", paymentId);
                throw;
            }
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByInvoiceIdAsync(int invoiceId)
        {
            try
            {
                return await _paymentRepository.GetPaymentByInvoiceIdAsync(invoiceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payments for InvoiceId {InvoiceId}", invoiceId);
                throw;
            }
        }

        public async Task<string> GetApprovalLinkAsync(string orderId)
        {
            try
            {
                return await _paymentProcessor.GetApprovalLinkAsync(orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving approval link for OrderId {OrderId}", orderId);
                throw;
            }
        }

        public async Task<string> CreatePayPalOrderAsync(decimal amount, string currency, Invoice invoice)
        {
            try
            {
                return await _paymentProcessor.CreatePayPalOrderAsync(amount, currency, invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating PayPal order for InvoiceId {InvoiceId}", invoice.InvoiceId);
                throw;
            }
        }
    }
}