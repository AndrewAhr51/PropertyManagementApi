using Microsoft.Extensions.Logging;
using PayPalCheckoutSdk.Orders;
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.DTOs.Payments.PayPal;
using PropertyManagementAPI.Domain.DTOs.Payments.Stripe;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Payments;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<PaymentService> _logger;
        private readonly IPayPalPaymentProcessor _payPalPaymentProcessor;
        private readonly PaymentAuditLogger _auditLogger;
        
        public PaymentService(IPaymentRepository paymentRepository,IInvoiceRepository invoiceRepository,ILogger<PaymentService> logger,
            IPayPalPaymentProcessor payPalPaymentProcessor, PaymentAuditLogger auditLogger)
        {
            _paymentRepository = paymentRepository;
            _invoiceRepository = invoiceRepository;
            _payPalPaymentProcessor = payPalPaymentProcessor;
            _logger = logger;
            _auditLogger = auditLogger;
        }

        public async Task<Payment> ProcessPaymentAsync(CreatePaymentDto dto)
        {
            try
            {
                return await _paymentRepository.ProcessPaymentAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment for InvoiceId {InvoiceId}", dto.InvoiceId);
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

        public async Task<bool> ReversePaymentAsync(int paymentId)
        {
            try
            {
                return await _paymentRepository.ReversePaymentAsync(paymentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reversing the payment for payment id {paymentId}", paymentId);
                throw;
            }

        }

    }
}