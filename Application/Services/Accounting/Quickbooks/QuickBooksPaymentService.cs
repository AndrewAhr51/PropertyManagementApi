using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Core;
using Intuit.Ipp.Security;

namespace PropertyManagementAPI.Application.Services.Accounting.Quickbooks
{
    public class QuickBooksPaymentService
    {
        private readonly QuickBooksTokenClient _tokenClient;

        public QuickBooksPaymentService(QuickBooksTokenClient tokenClient)
        {
            _tokenClient = tokenClient;
        }

        public async Task<Payment> CreatePaymentAsync(string realmId, string customerId, string invoiceId, decimal amount)
        {
            var token = await _tokenClient.GetBearerTokenAsync();
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
                                TxnType = TxnTypeEnum.Invoice.ToString() // Fix: Convert TxnTypeEnum to string using ToString()
                            }
                        }
                    }
                }
            };

            return dataService.Add(payment) as Payment;
        }

    }
}
