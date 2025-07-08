using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Application.Services.Payments.Stripe;
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.DTOs.Payments.Stripe;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;
using Stripe;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments
{
    public class StripeRepository : IStripeRepository
    {
        private readonly MySqlDbContext _context;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<StripeRepository> _logger;
        private readonly AuditEventBuilder _audit;

        public StripeRepository(
            MySqlDbContext context,
            IInvoiceRepository invoiceRepository,
            ILogger<StripeRepository> logger,
            AuditEventBuilder audit)
        {
            _context = context;
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _audit = audit;
        }

        public async Task<StripePaymentResponseDto> CreateStripePaymentIntentAsync(CreateStripeDto dto)
        {
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceByIdAsync(dto.InvoiceId)
                    ?? throw new ArgumentException($"Invoice {dto.InvoiceId} not found");

                var intent = await ProcessPaymentIntentAsync(dto.Amount, dto.Currency);

                var responseDto = new StripePaymentResponseDto
                {
                    ClientSecret = intent.ClientSecret,
                    Status = intent.Status,
                    Amount = dto.Amount,
                    Currency = dto.Currency,
                    InvoiceId = invoice.InvoiceId,
                    InvoiceReference = invoice.ReferenceNumber
                };

                await _audit.LogSuccessAsync("CreateStripePaymentIntentAsync", new
                {
                    dto.InvoiceId,
                    dto.TenantId,
                    dto.Amount
                }, "Web");

                return responseDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed: CreateStripePaymentIntentAsync for InvoiceId {InvoiceId}", dto.InvoiceId);

                await _audit.LogFailureAsync("CreateStripePaymentIntentAsync", new
                {
                    dto.InvoiceId,
                    dto.TenantId,
                    dto.Amount,
                    Error = ex.Message
                }, "Web");

                throw;
            }
        }

        public async Task<StripePaymentResponseDto> ProcessPaymentIntentAsync(decimal amount, string currency)
        {
            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(amount * 100), // Stripe uses cents
                    Currency = currency.ToLower(),
                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = new Dictionary<string, string>
            {
                { "Application", "PropertyManagement" },
                { "Purpose", "InvoicePayment" }
            }
                };

                var service = new PaymentIntentService();
                var intent = await service.CreateAsync(options);

                return new StripePaymentResponseDto
                {
                    ClientSecret = intent.ClientSecret,
                    Status = intent.Status,
                    Amount = amount,
                    Currency = currency
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe API error while creating payment intent.");
                throw new ApplicationException("Stripe payment initialization failed", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during ProcessPaymentIntentAsync.");
                throw;
            }
        }
        public async Task<bool> CreateStripePaymentAsync(CreateStripeDto dto)
        {
            return await HandlePaymentAsync(dto, "CreateStripePaymentAsync");
        }

        public async Task<bool> ProcessStripePaymentAsync(CreateStripeDto dto)
        {
            return await HandlePaymentAsync(dto, "ProcessStripePaymentAsync");
        }

        private async Task<bool> HandlePaymentAsync(CreateStripeDto dto, string action)
        {
            try
            {
                var payment = BuildCardPaymentFromDto(dto);

                await AddPaymentAsync(payment);
                await SavePaymentChangesAsync();

                _logger.LogInformation("✅ {Action} created Stripe card payment: {ReferenceNumber}", action, payment.ReferenceNumber);

                await _audit.LogSuccessAsync(action, new
                {
                    dto.InvoiceId,
                    dto.TenantId,
                    dto.Amount
                }, "Web");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed: {Action} for InvoiceId {InvoiceId}", action, dto.InvoiceId);

                await _audit.LogFailureAsync(action, new
                {
                    dto.InvoiceId,
                    dto.TenantId,
                    dto.Amount,
                    Error = ex.Message
                }, "Web");

                return false;
            }
        }

        private CardPayment BuildCardPaymentFromDto(CreateStripeDto dto)
        {
            return new CardPayment
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
        }

        public async Task FinalizePaymentAsync(Payment payment, Dictionary<string, string> metadata)
        {
            await AddPaymentAsync(payment);

            if (metadata?.Any() == true)
            {
                foreach (var (key, value) in metadata)
                {
                    _context.PaymentMetadata.Add(new PaymentMetadata
                    {
                        Payment = payment,
                        Key = key,
                        Value = value
                    });
                }
            }

            var invoice = await _context.InvoiceDocuments.FindAsync(payment.InvoiceId);
            if (invoice != null)
            {
                invoice.IsPaid = true;
                _context.InvoiceDocuments.Attach(invoice);
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