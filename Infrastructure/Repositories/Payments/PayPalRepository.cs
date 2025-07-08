using PropertyManagementAPI.Application.Services.Payments;
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.DTOs.Payments.PayPal;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments
{
    public class PayPalRepository : IPayPalRepository
    {
        private readonly MySqlDbContext _context;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<PayPalRepository> _logger;
        private readonly IPayPalPaymentProcessor _paymentProcessor;
        private readonly PaymentAuditLogger _auditLogger;

        public PayPalRepository(MySqlDbContext context, IInvoiceRepository invoiceRepository, ILogger<PayPalRepository> logger,
            IPayPalPaymentProcessor paymentProcessor, PaymentAuditLogger auditLogger)
        {

            _context = context;
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _paymentProcessor = paymentProcessor;
            _auditLogger = auditLogger;

        }
        
        // Step 1: Create PayPal Order
        public async Task<PayPalApprovalDto> CreatePayPalOrderAsync(CreatePayPalDto dto)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(dto.InvoiceId);
            if (invoice == null || invoice.InvoiceId != invoice.InvoiceId)
                throw new InvalidOperationException("Invalid invoice or tenant mismatch.");

            var idempotencyKey = $"paypal:{invoice.TenantId}:{dto.InvoiceId}:{DateTime.UtcNow:yyyyMMddHHmmssfff}";

            var orderResult = await _paymentProcessor.CreatePayPalOrderAsync(invoice.Amount, "USD", invoice, idempotencyKey);

            await _auditLogger.LogAsync(
                action: "CreatePayPalOrder",
                status: "SUCCESS",
                response: new
                {
                    OrderId = orderResult.OrderId,
                    ApprovalLink = orderResult.ApprovalLink,
                    InvoiceId = invoice.InvoiceId,
                    Amount = invoice.Amount,
                    Currency = "USD",
                    IdempotencyKey = idempotencyKey
                },
                performedBy: "Web"
            );

            return new PayPalApprovalDto
            {
                OrderId = orderResult.OrderId,
                ApprovalLink = orderResult.ApprovalLink
            };
        }

        // Step 2: Capture PayPal Order
        public async Task<CardPayment> CapturePayPalCardPaymentAsync(CreatePayPalDto dto)
        {
            var invoice = await _invoiceRepository.GetInvoiceByIdAsync(dto.InvoiceId);
            if (invoice == null || invoice.TenantId != dto.TenantId)
                throw new InvalidOperationException("Invalid invoice or tenant mismatch.");

            var status = await _paymentProcessor.CapturePayPalOrderAsync(dto.OrderId);

            await _auditLogger.LogAsync(
                action: "CaptureOrder",
                status: status,
                response: new { OrderId = dto.OrderId, Amount = dto.Amount, Currency = "USD" },
                performedBy: "Web"
            );

            if (status != "COMPLETED")
                throw new InvalidOperationException($"Card payment failed with status: {status}");

            return new CardPayment
            {
                OrderId = dto.OrderId,
                Status = status,
                CardType = dto.Metadata.GetValueOrDefault("CardType"),
                Last4Digits = dto.Metadata.GetValueOrDefault("Last4Digits"),
                AuthorizationCode = dto.OrderId,

                Amount = invoice.Amount,
                PaidOn = dto.PaymentDate,
                InvoiceId = dto.InvoiceId,
                TenantId = dto.TenantId == 0 ? null : dto.TenantId,
                OwnerId = dto.OwnerId == 0 ? null : dto.OwnerId,
                PaymentType = "PayPal",
                ReferenceNumber = ReferenceNumberHelper.Generate("REF", invoice.PropertyId),
            };
        }

        public async Task<PayPalPaymentResponseDto> ProcessPayPalPaymentAsync(CreatePayPalDto dto)
        {
            PayPalPaymentResponseDto payPalResponse = new PayPalPaymentResponseDto();

            try
            {
                var invoice = await _invoiceRepository.GetInvoiceByIdAsync(dto.InvoiceId);
                if (invoice == null || invoice.TenantId != dto.TenantId)
                    throw new InvalidOperationException("Invalid invoice or tenant mismatch.");

                var cardPayment = await CapturePayPalCardPaymentAsync(dto);

                if (invoice.OwnerId == 0) invoice.OwnerId = null;

                payPalResponse.OrderId = cardPayment.OrderId;
                payPalResponse.Status = cardPayment.Status;
                payPalResponse.Amount = invoice.Amount;
                payPalResponse.CurrencyCode = "USD";
                payPalResponse.InvoiceId = dto.InvoiceId;
                payPalResponse.InvoiceReference = invoice.ReferenceNumber;
                
                Payment payment = new CardPayment
                {
                    OrderId = cardPayment.OrderId,
                    Status = cardPayment.Status,
                    CardType = cardPayment.CardType,
                    Last4Digits = cardPayment.Last4Digits,
                    AuthorizationCode = cardPayment.AuthorizationCode,
                    Amount = invoice.Amount,
                    PaidOn = dto.PaymentDate,
                    InvoiceId = dto.InvoiceId,
                    TenantId = dto.TenantId,
                    OwnerId = dto.OwnerId,
                    PaymentType = "PayPal",
                    ReferenceNumber = ReferenceNumberHelper.Generate("REF", invoice.PropertyId)
                };

                await FinalizePaymentAsync(payment, dto.Metadata);

                _logger.LogInformation("Payment created successfully: {ReferenceNumber}", payment.ReferenceNumber);

                return payPalResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment for InvoiceId {InvoiceId}, TenantId {TenantId}", dto.InvoiceId, dto.TenantId);
                throw;
            }
        }

        public async Task FinalizePaymentAsync(Payment payment, Dictionary<string, string> metadata)
        {
            await AddPaymentAsync(payment);

            if (metadata?.Any() == true)
            {
                foreach (var kvp in metadata)
                {
                    _context.PaymentMetadata.Add(new PaymentMetadata
                    {
                        Payment = payment,
                        Key = kvp.Key,
                        Value = kvp.Value
                    });
                }
            }

            var invoice = await _context.InvoiceDocuments.FindAsync(payment.InvoiceId);
            if (invoice != null)
            {
                _context.InvoiceDocuments.Attach(invoice);
                invoice.IsPaid = true;
                _context.Entry(invoice).Property(i => i.IsPaid).IsModified = true;
            }

            var tenant = await _context.Tenants.FindAsync(payment.TenantId);
            if (tenant != null)
            {
                tenant.Balance += payment.Amount < 0 ? payment.Amount : -payment.Amount;
                _context.Tenants.Update(tenant);
            }

            await SavePaymentChangesAsync();
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }

        public async Task SavePaymentChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


    }
}
