using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Core;
using Intuit.Ipp.Security;
using Microsoft.Extensions.Logging;

namespace PropertyManagementAPI.Application.Services.Accounting.Quickbooks
{
    public class QuickBooksPaymentService
    {
        private readonly QuickBooksTokenClient _tokenClient;
        private readonly ILogger<QuickBooksPaymentService> _logger;

        public QuickBooksPaymentService(
            QuickBooksTokenClient tokenClient,
            ILogger<QuickBooksPaymentService> logger)
        {
            _tokenClient = tokenClient;
            _logger = logger;
        }

        public async Task<Payment> CreatePaymentAsync(string realmId, string customerId, string invoiceId, decimal amount)
        {
            try
            {
                var token = await _tokenClient.GetBearerTokenAsync();
                if (string.IsNullOrWhiteSpace(token))
                {
                    _logger.LogError("Access token retrieval failed for realm {RealmId}.", realmId);
                    throw new InvalidOperationException("Missing QuickBooks access token.");
                }

                var validator = new OAuth2RequestValidator(token);
                var context = new ServiceContext(realmId, IntuitServicesType.QBO, validator);
                var dataService = new DataService(context);

                var payment = new Payment
                {
                    CustomerRef = new ReferenceType { Value = customerId },
                    TotalAmt = amount,
                    Line = new[]
                    {
                        new Line
                        {
                            Amount = amount,
                            LinkedTxn = new[]
                            {
                                new LinkedTxn
                                {
                                    TxnId = invoiceId,
                                    TxnType = TxnTypeEnum.Invoice.ToString()
                                }
                            }
                        }
                    }
                };

                var result = dataService.Add(payment) as Payment;

                if (result == null)
                {
                    _logger.LogError("Failed to create payment for customer {CustomerId} on invoice {InvoiceId} in realm {RealmId}.",
                        customerId, invoiceId, realmId);
                    throw new InvalidOperationException("Payment creation failed.");
                }

                _logger.LogInformation("Payment successfully created for customer {CustomerId} in realm {RealmId}.",
                    customerId, realmId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment for customer {CustomerId} on invoice {InvoiceId} in realm {RealmId}.",
                    customerId, invoiceId, realmId);
                throw;
            }
        }
    }
}