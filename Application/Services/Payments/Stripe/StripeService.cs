using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.DTOs.Payments.Stripe;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Payments;
using Stripe;

namespace PropertyManagementAPI.Application.Services.Payments.Stripe
{
    public class StripeService : IStripeService
    {
        private readonly string _secretKey;
        private readonly string _publishableKey;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogger<StripeService> _logger;
        private readonly IStripeRepository _stripeRepository;
        private readonly PaymentAuditLogger _auditLogger;

        public StripeService(string secretKey, string publishableKey, IInvoiceRepository invoiceRepository, ILogger<StripeService> logger,
              IStripeRepository stripeRepository,PaymentAuditLogger auditLogger)
        {
            _secretKey = secretKey;
            _publishableKey = publishableKey;
            _invoiceRepository = invoiceRepository;
            _logger = logger;
            _stripeRepository = stripeRepository;
            _auditLogger = auditLogger;
                  
            StripeConfiguration.ApiKey = _secretKey;
        }
             
        public async Task<PaymentIntent> ProcessPaymentIntentAsync(decimal amount, string currency)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)amount,
                Currency = currency,
                PaymentMethodTypes = new List<string> { "card" }
            };

            var service = new PaymentIntentService();
            return await service.CreateAsync(options);
        }

        public async Task<StripePaymentResponseDto> ProcessStripePaymentAsync(CreateStripeDto dto)
        {
            StripePaymentResponseDto stripeDto = new StripePaymentResponseDto();
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceByIdAsync(dto.InvoiceId);
                if (invoice == null)
                    throw new ArgumentException($"Invoice {dto.InvoiceId} not found");

                var intent = await ProcessPaymentIntentAsync(dto.Amount, dto.Currency);

                stripeDto = new StripePaymentResponseDto
                {
                    ClientSecret = intent.ClientSecret,
                    Status = intent.Status,
                    Amount = dto.Amount,
                    Currency = dto.Currency,
                    InvoiceId = invoice.InvoiceId,
                    InvoiceReference = invoice.ReferenceNumber
                };

                var save = await _stripeRepository.ProcessStripePaymentAsync(dto);
                if (!save)
                {
                    throw new InvalidOperationException("Failed to save Stripe payment details.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Stripe payment intent for InvoiceId {InvoiceId}", dto.InvoiceId);
                throw;
            }

            return stripeDto;

        }

        public async Task<StripePaymentResponseDto> CreateStripePaymentIntentAsync(CreateStripeDto dto)
        {
            StripePaymentResponseDto stripeDto = new StripePaymentResponseDto();
            try
            {
                var invoice = await _invoiceRepository.GetInvoiceByIdAsync(dto.InvoiceId);
                if (invoice == null)
                    throw new ArgumentException($"Invoice {dto.InvoiceId} not found");

                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)dto.Amount,
                    Currency = dto.Currency,
                    PaymentMethodTypes = new List<string> { "card" }
                };

                var service = new PaymentIntentService();
                var intent = await service.CreateAsync(options);

                stripeDto = new StripePaymentResponseDto
                {
                    ClientSecret = intent.ClientSecret,
                    Status = intent.Status,
                    Amount = dto.Amount,
                    Currency = dto.Currency,
                    InvoiceId = invoice.InvoiceId,
                    InvoiceReference = invoice.ReferenceNumber
                };

                var save = await _stripeRepository.CreateStripePaymentAsync(dto);
                if (!save)
                {
                    throw new InvalidOperationException("Failed to save Stripe payment details.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Stripe payment intent for InvoiceId {InvoiceId}", dto.InvoiceId);
                throw;
            }

            return stripeDto;

        }

    }
}
