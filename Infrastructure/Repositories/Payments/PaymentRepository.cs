using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Application.Services.Payments;
using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;
using PropertyManagementAPI.Application.Services.Payments.Stripe;
using PropertyManagementAPI.Domain.DTOs.Payments.PayPal;
using PropertyManagementAPI.Domain.DTOs.Payments.Stripe;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;
using PropertyManagementAPI.Domain.Entities.Payments.Banking;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly MySqlDbContext _context;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<PaymentService> _logger;
        private readonly IPaymentProcessor _paymentProcessor;
        private readonly PaymentAuditLogger _auditLogger;
        private readonly IStripeService _stripeService;

        public PaymentRepository(MySqlDbContext context, IInvoiceRepository invoiceRepository, ILogger<PaymentService> logger,
                IPaymentProcessor paymentProcessor, PaymentAuditLogger auditLogger, IStripeService stripeService )
        {
            _context = context;
            _invoiceRepository = invoiceRepository;
            _paymentProcessor = paymentProcessor;
            _logger = logger;
            _auditLogger = auditLogger;
            _stripeService = stripeService;
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

                if (dto.OwnerId == 0) dto.OwnerId = null;
                if (dto.TenantId == 0) dto.TenantId = null;

                payment.Amount = dto.Amount;
                payment.PaidOn = dto.PaidOn;
                payment.InvoiceId = dto.InvoiceId;
                payment.TenantId = dto.TenantId;
                payment.OwnerId = dto.OwnerId;
                payment.PaymentType = dto.PaymentMethod;
                payment.ReferenceNumber = ReferenceNumberHelper.Generate("REF", invoice.PropertyId);

                await AddPaymentAsync(payment);
                await SavePaymentChangesAsync();

                _logger.LogInformation("Payment created successfully: {ReferenceNumber}", payment.ReferenceNumber);

                return payment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment for InvoiceId {InvoiceId}, TenantId {TenantId}", dto.InvoiceId, dto.TenantId);
                throw;
            }
        }

        public async Task<PayPalPaymentResponseDto> CreatePayPalPaymentAsync(CreatePayPalDto dto)
        {
            PayPalPaymentResponseDto payPalResponse = new PayPalPaymentResponseDto();

            try
            {
                var invoice = await _invoiceRepository.GetInvoiceByIdAsync(dto.InvoiceId);
                if (invoice == null || invoice.TenantId != dto.TenantId)
                    throw new InvalidOperationException("Invalid invoice or tenant mismatch.");

                var cardPayment = await ProcessPayPalCardPaymentAsync(dto, invoice);

                if (dto.OwnerId == 0) dto.OwnerId = null;
                if (dto.TenantId == 0) dto.TenantId = null;

                payPalResponse.OrderId = cardPayment.OrderId;
                payPalResponse.Status = cardPayment.Status;
                payPalResponse.Amount = dto.Amount;
                payPalResponse.CurrencyCode = "USD";
                payPalResponse.InvoiceId = dto.InvoiceId;
                payPalResponse.InvoiceReference = invoice.ReferenceNumber;
                payPalResponse.ProcessedAt = DateTime.UtcNow;
                payPalResponse.GatewayTransactionId =
                payPalResponse.Note = invoice.Notes;
                payPalResponse.PerformedBy = "Web";

                Payment payment = new CardPayment
                {
                    OrderId = cardPayment.OrderId,
                    Status = cardPayment.Status,
                    CardType = cardPayment.CardType,
                    Last4Digits = cardPayment.Last4Digits,
                    AuthorizationCode = cardPayment.AuthorizationCode,
                    Amount = dto.Amount,
                    PaidOn = dto.PaymentDate,
                    InvoiceId = dto.InvoiceId,
                    TenantId = dto.TenantId,
                    OwnerId = dto.OwnerId,
                    PaymentType = "PayPal",
                    
                    ReferenceNumber = ReferenceNumberHelper.Generate("REF", invoice.PropertyId)
                };
               
                await AddPaymentAsync(payment);
                await SavePaymentChangesAsync();

                _logger.LogInformation("Payment created successfully: {ReferenceNumber}", payment.ReferenceNumber);

                return payPalResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment for InvoiceId {InvoiceId}, TenantId {TenantId}", dto.InvoiceId, dto.TenantId);
                throw;
            }
        }

        private async Task<CardPayment> ProcessPayPalCardPaymentAsync(CreatePayPalDto dto, Invoice invoice)
        {
            try
            {
                var orderId = await _paymentProcessor.CreatePayPalOrderAsync(dto.Amount, "USD", invoice);
                var status = await _paymentProcessor.CapturePayPalOrderAsync(orderId);

                // Log success BEFORE persisting
                await _auditLogger.LogAsync(
                    action: "CaptureOrder",
                    status: status,
                    response: new { OrderId = orderId, Amount = dto.Amount, Currency = "USD" },
                    performedBy: dto.PerformedBy ?? "System"
                );

                if (status != "COMPLETED")
                    throw new InvalidOperationException($"Card payment failed with status: {status}");

                return new CardPayment
                {
                    OrderId = orderId,
                    Status = status,
                    CardType = dto.Metadata.GetValueOrDefault("CardType"),
                    Last4Digits = dto.Metadata.GetValueOrDefault("Last4Digits"),
                    AuthorizationCode = orderId
                };
            }
            catch (Exception ex)
            {
                await _auditLogger.LogAsync(
                    action: "CaptureOrder",
                    status: "FAILED",
                    response: new { Error = ex.Message, dto.Amount, InvoiceId = invoice.InvoiceId },
                    performedBy: dto.PerformedBy ?? "System"
                );

                throw; // Rethrow for upstream handling
            }
        }

        public async Task<Payment> GetPaymentByIdAsync(int paymentId)
        {
            var payments = await _context.Payments
                .Include(p => p.Invoice)
                .Include(p => p.Tenant)
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);

            if (payments == null)
            {
                throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");
            }

            return payments;
        }

        public async Task<IEnumerable<Payment>> GetPaymentByInvoiceIdAsync(int invoiceId)
        {
            return await _context.Payments
                .Where(p => p.InvoiceId == invoiceId)
                .ToListAsync();
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }
   

        public async Task<StripePaymentResponseDto> CreateStripePaymentIntentAsync(CreateStripeDto dto)
        {
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceByIdAsync(dto.InvoiceId);
                if (invoice == null)
                    throw new ArgumentException($"Invoice {dto.InvoiceId} not found");

                var intent = await _stripeService.CreatePaymentIntentAsync(dto.Amount, dto.Currency);

                return new StripePaymentResponseDto
                {
                    ClientSecret = intent.ClientSecret,
                    Status = intent.Status,
                    Amount = dto.Amount,
                    Currency = dto.Currency,
                    InvoiceId = invoice.InvoiceId,
                    InvoiceReference = invoice.ReferenceNumber
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Stripe payment intent for InvoiceId {InvoiceId}", dto.InvoiceId);
                throw;
            }
        }

        public async Task<bool> CreateStripePaymentAsync(CreateStripeDto dto)
        {
            try
            {
                var payment = new CardPayment
                {
                    CardType = dto.Metadata.GetValueOrDefault("CardType"),
                    Last4Digits = dto.Metadata.GetValueOrDefault("Last4Digits"),
                    AuthorizationCode = dto.Metadata.GetValueOrDefault("AuthorizationCode"),
                    Amount = dto.Amount,
                    PaidOn = dto.PaidOn,
                    InvoiceId = dto.InvoiceId,
                    TenantId = dto.TenantId == 0 ? null : dto.TenantId,
                    OwnerId = dto.OwnerId == 0 ? null : dto.OwnerId,
                    PaymentType = "Card",
                    ReferenceNumber = ReferenceNumberHelper.Generate("REF", dto.PropertyId)
                };

                await AddPaymentAsync(payment);
                await SavePaymentChangesAsync();

                _logger.LogInformation("Stripe card payment created: {ReferenceNumber}", payment.ReferenceNumber);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Stripe card payment for InvoiceId {InvoiceId}", dto.InvoiceId);
                return false;
            }
        }

        public async Task SavePaymentChangesAsync()
{
    await _context.SaveChangesAsync();
}
    }
}
