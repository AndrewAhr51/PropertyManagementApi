using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.DTOs.Payments;
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


        public PaymentService(IPaymentRepository paymentRepo, IInvoiceRepository invoiceRepo, ILogger<PaymentService> logger)
        {
            _paymentRepository = paymentRepo;
            _invoiceRepository = invoiceRepo;
            _logger = logger;
        }
        public async Task<Payment> CreatePaymentAsync(CreatePaymentDto dto)
        {
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceByIdAsync(dto.InvoiceId);
                if (invoice == null || invoice.TenantId != dto.TenantId)
                    throw new InvalidOperationException("Invalid invoice or tenant mismatch.");

                Payment payment = dto.PaymentMethod switch
                {
                    "Card" => new CardPayment
                    {
                        CardType = dto.Metadata.GetValueOrDefault("CardType"),
                        Last4Digits = dto.Metadata.GetValueOrDefault("Last4Digits"),
                        AuthorizationCode = dto.Metadata.GetValueOrDefault("AuthorizationCode")
                    },
                    "Check" => new CheckPayment
                    {
                        CheckNumber = dto.Metadata.GetValueOrDefault("CheckNumber"),
                        CheckBankName = dto.Metadata.GetValueOrDefault("BankName")
                    },
                    "Transfer" => new ElectronicTransferPayment
                    {
                        BankAccountNumber = dto.Metadata.GetValueOrDefault("BankAccountNumber"),
                        RoutingNumber = dto.Metadata.GetValueOrDefault("RoutingNumber"),
                        TransactionId = dto.Metadata.GetValueOrDefault("TransactionId")
                    },
                    _ => throw new NotSupportedException($"Unsupported payment method: {dto.PaymentMethod}")
                };

                if (dto.OwnerId == 0)
                {
                    dto.OwnerId = null;
                }

                if (dto.TenantId == 0)
                {
                    dto.TenantId = null;
                }

                payment.Amount = dto.Amount;
                payment.PaidOn = dto.PaidOn;
                payment.InvoiceId = dto.InvoiceId;
                payment.TenantId = dto.TenantId;
                payment.OwnerId = dto.OwnerId;
                payment.PaymentType = dto.PaymentMethod;
                payment.ReferenceNumber = ReferenceNumberHelper.Generate("REF", invoice.PropertyId);

                await _paymentRepository.AddAsync(payment);
                await _paymentRepository.SaveChangesAsync();

                _logger.LogInformation("Payment created successfully: {ReferenceNumber}", payment.ReferenceNumber);

                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment for InvoiceId {InvoiceId}, TenantId {TenantId}", dto.InvoiceId, dto.TenantId);
                throw;
            }
        }

        public async Task<Payment> GetPaymentByIdAsync(int paymentId)
        {
            try
            {
                return await _paymentRepository.GetByIdAsync(paymentId);
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
                return await _paymentRepository.GetByInvoiceIdAsync(invoiceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payments for InvoiceId {InvoiceId}", invoiceId);
                throw;
            }
        }
    }
}
