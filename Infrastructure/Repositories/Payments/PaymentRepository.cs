using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Application.Services.Payments;
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Domain.Entities.Invoices.Base;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Domain.Entities.Payments.Banking;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly MySqlDbContext _context;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<PaymentService> _logger;
        private readonly PaymentAuditLogger _auditLogger;

        public PaymentRepository(MySqlDbContext context, IInvoiceRepository invoiceRepository, ILogger<PaymentService> logger,
               PaymentAuditLogger auditLogger)
        {
            _context = context;
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _auditLogger = auditLogger;
        }

        public async Task<Payment> ProcessPaymentAsync(CreatePaymentDto dto)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var invoice = await _context.InvoiceDocuments.FirstOrDefaultAsync(i => i.InvoiceId == dto.InvoiceId);
                if (invoice == null || invoice.TenantId != dto.TenantId)
                    throw new InvalidOperationException("Invalid invoice or tenant mismatch.");

                if (invoice.IsPaid)
                    throw new InvalidOperationException("Cannot create payment for an already paid invoice.");

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

                // ⬇️ Persist metadata records, if needed
                if (dto.Metadata?.Any() == true)
                {
                    foreach (var kvp in dto.Metadata)
                    {
                        var metadata = new PaymentMetadata
                        {
                            Payment = payment,
                            Key = kvp.Key,
                            Value = kvp.Value
                        };
                        _context.PaymentMetadata.Add(metadata);
                    }
                }

                await FinalizePaymentAsync(payment, dto.Metadata);
                await SavePaymentChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Payment created successfully: {ReferenceNumber}", payment.ReferenceNumber);

                return payment;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error creating payment for InvoiceId {InvoiceId}, TenantId {TenantId}", dto.InvoiceId, dto.TenantId);
                throw;
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

        public async Task SavePaymentChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ReversePaymentAsync(int paymentId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var original = await _context.Payments.FindAsync(paymentId);
                if (original == null || original.Amount <= 0)
                    return false;

                var invoice = await _context.InvoiceDocuments.FirstOrDefaultAsync(i => i.InvoiceId == original.InvoiceId);
                if (invoice == null || invoice.TenantId != original.TenantId)
                    throw new InvalidOperationException("Invalid invoice or tenant mismatch.");

                var paymentAmount = -original.Amount;

                var reversal = new Payment
                {
                    Amount = paymentAmount,
                    ReferenceNumber = $"REV-{original.ReferenceNumber}",
                    InvoiceId = original.InvoiceId,
                    TenantId = original.TenantId,
                    OwnerId = original.OwnerId,
                    PaymentType = original.PaymentType,
                    CardType = original.CardType,
                    Last4Digits = original.Last4Digits,
                    AuthorizationCode = original.AuthorizationCode,
                    CheckNumber = original.CheckNumber,
                    CheckBankName = original.CheckBankName,
                    TransactionId = original.TransactionId
                };

                _context.Payments.Add(reversal);

                // ⬇️ Mark invoice as unpaid
                _context.InvoiceDocuments.Attach(invoice);
                invoice.IsPaid = false;
                _context.Entry(invoice).Property(x => x.IsPaid).IsModified = true;

                // 💰 Adjust tenant balance
                var tenant = await _context.Tenants.FindAsync(original.TenantId);
                if (tenant != null)
                {
                    if (tenant.Balance + (paymentAmount * -1) < 0)
                        throw new InvalidOperationException("Insufficient tenant balance to reverse payment.");
                    
                    //Multiply the payment by negarive one becasue we will always ADD back to balance.
                    tenant.Balance += paymentAmount * -1;
                    _context.Tenants.Update(tenant);
                }

                var save = await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return save > 0;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error reversing payment with PaymentId {PaymentId}", paymentId);
                return false;
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
    }
}
