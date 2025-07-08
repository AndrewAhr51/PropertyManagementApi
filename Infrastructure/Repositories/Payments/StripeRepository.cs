using PropertyManagementAPI.Application.Services.Payments;
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.DTOs.Payments.Stripe;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments
{
    public class StripeRepository : IStripeRepository
    {
        private readonly MySqlDbContext _context;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<StripeRepository> _logger;
        private readonly IStripeService _stripeService;
        private readonly AuditEventBuilder _audit;

        public StripeRepository(MySqlDbContext context, IInvoiceRepository invoiceRepository, ILogger<StripeRepository> logger, IStripeService stripeService, AuditEventBuilder audit)
        {
            _context = context;
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _stripeService = stripeService;
            _audit = audit;

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

        public async Task<StripePaymentResponseDto> CreateStripePaymentIntentAsync(CreateStripeDto dto)
        {
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceByIdAsync(dto.InvoiceId);
                if (invoice == null)
                    throw new ArgumentException($"Invoice {dto.InvoiceId} not found");

                var intent = await _stripeService.ProcessPaymentIntentAsync(dto.Amount, dto.Currency);

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
                    InvoiceId = dto.InvoiceId,
                    TenantId = dto.TenantId,
                    Amount = dto.Amount
                }, "Web");


                return responseDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Stripe payment intent for InvoiceId {InvoiceId}", dto.InvoiceId);

                await _audit.LogFailureAsync("CreateStripePaymentIntentAsync", new
                {
                    InvoiceId = dto.InvoiceId,
                    TenantId = dto.TenantId,
                    Amount = dto.Amount,
                    Error = ex.Message
                }, "Web");

                throw;
            }
        }

        public async Task<bool> ProcessStripePaymentAsync(CreateStripeDto dto)
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

                await _audit.LogSuccessAsync("ProcessStripePaymentAsync", new
                {
                    InvoiceId = dto.InvoiceId,
                    TenantId = dto.TenantId,
                    Amount = dto.Amount
                }, "Web");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Stripe card payment for InvoiceId {InvoiceId}", dto.InvoiceId);

                await _audit.LogFailureAsync("ProcessStripePaymentAsync", new
                {
                    InvoiceId = dto.InvoiceId,
                    TenantId = dto.TenantId,
                    Amount = dto.Amount,
                    Error = ex.Message
                }, "Web");

                return false;
            }
        }

        public async Task SavePaymentChangesAsync()
        {
            await _context.SaveChangesAsync();
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
    }
}
