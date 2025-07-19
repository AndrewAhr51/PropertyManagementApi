using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.DTOs.Payments.Stripe;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Payments;
using Stripe;
using Stripe.Checkout;

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

        public async Task<StripeSessionDto?> GetSessionAsync(string sessionId)
        {
            try
            {
                var sessionService = new SessionService();
                var sessionOptions = new SessionGetOptions
                {
                    Expand = new List<string>
            {
                "payment_intent",
                "customer"
            }
                };

                var session = await sessionService.GetAsync(sessionId, sessionOptions);
                var paymentIntentId = session.PaymentIntent?.Id;

                if (string.IsNullOrEmpty(paymentIntentId))
                {
                    _logger.LogWarning("⚠️ No PaymentIntent ID found for session: {SessionId}", sessionId);
                    return null;
                }

                // 🔍 Retrieve charges manually using ChargeService
                var chargeService = new ChargeService();
                var chargeList = await chargeService.ListAsync(new ChargeListOptions
                {
                    PaymentIntent = paymentIntentId
                });

                var charge = chargeList?.Data?.FirstOrDefault();
                var card = charge?.PaymentMethodDetails?.Card;
                var address = session.CustomerDetails?.Address;

                return new StripeSessionDto
                {
                    SessionId = session.Id,
                    AmountTotal = session.AmountTotal ?? 0,
                    Currency = session.Currency ?? "USD",
                    ReceiptUrl = charge?.ReceiptUrl ?? string.Empty,
                    CardBrand = card?.Brand ?? string.Empty,
                    CardLast4 = card?.Last4 ?? string.Empty,
                    ExpMonth = card?.ExpMonth.ToString() ?? string.Empty,
                    ExpYear = card?.ExpYear.ToString() ?? string.Empty,
                    ChargeStatus = charge?.Status ?? string.Empty,
                    StripeChargeId = charge?.Id ?? string.Empty,
                    Metadata = new StripeSessionMetadata
                    {
                        InvoiceId = session.Metadata.TryGetValue("invoiceId", out var invoice) ? invoice : "—",
                        TenantId = session.Metadata.TryGetValue("tenantId", out var tenant) ? tenant : "—",
                        TenantName = session.Metadata.TryGetValue("tenantName", out var tenantName) ? tenantName : "—",
                        PropertyId = session.Metadata.TryGetValue("propertyId", out var property) ? property : "—",
                        PropertyName = session.Metadata.TryGetValue("propertyName", out var propertyName) ? propertyName : "—",
                        OwnerId = session.Metadata.TryGetValue("ownerId", out var owner) ? owner : "—"
                    },
                    Address = new StripeBillingAddress
                    {
                        Line1 = address?.Line1 ?? string.Empty,
                        Line2 = address?.Line2 ?? string.Empty,
                        City = address?.City ?? string.Empty,
                        State = address?.State ?? string.Empty,
                        PostalCode = address?.PostalCode ?? string.Empty,
                        Country = address?.Country ?? string.Empty
                    }
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "❌ Stripe error while fetching session: {SessionId}", sessionId);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Unexpected error while fetching session: {SessionId}", sessionId);
                return null;
            }
        }


    }
}
