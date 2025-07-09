using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Security;
using PropertyManagementAPI.Application.Services.Accounting.Quickbooks;
using PropertyManagementAPI.Domain.DTOs.Invoices;
using Customer = Intuit.Ipp.Data.Customer;
using Invoice = Intuit.Ipp.Data.Invoice;
using Line = Intuit.Ipp.Data.Line;
using ReferenceType = Intuit.Ipp.Data.ReferenceType;

namespace PropertyManagementAPI.Infrastructure.Quickbooks
{
    public class QuickBooksInvoiceService : IQuickBooksInvoiceService
    {
        private readonly IQuickBooksTokenManager _tokenManager;
        private readonly ILogger<QuickBooksInvoiceService> _logger;

        public QuickBooksInvoiceService(
            IQuickBooksTokenManager tokenManager,
            ILogger<QuickBooksInvoiceService> logger)
        {
            _tokenManager = tokenManager;
            _logger = logger;
        }

        public async Task<Invoice> CreateInvoiceAsync(string realmId, Customer customer, decimal amount, string itemId)
        {
            var token = await _tokenManager.GetTokenAsync(realmId); // consider tenant-aware token access
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogError("Access token for realm {RealmId} is missing or invalid.", realmId);
                throw new InvalidOperationException("Missing QuickBooks access token.");
            }

            var serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, new OAuth2RequestValidator(token));
            var dataService = new DataService(serviceContext);

            var lineItem = new Line
            {
                Amount = amount,
                DetailType = LineDetailTypeEnum.SalesItemLineDetail,
                Description = "Generated invoice line",
                AnyIntuitObject = new SalesItemLineDetail
                {
                    ItemRef = new ReferenceType { Value = itemId }
                }
            };

            var invoice = new Invoice
            {
                CustomerRef = new ReferenceType { Value = customer.Id },
                Line = new Line[] { lineItem }
            };

            return dataService.Add(invoice) as Invoice
                ?? throw new InvalidOperationException("Invoice creation failed.");
        }

        public async Task<bool> VerifyConnectionAsync(string realmId)
        {
            try
            {
                var token = await _tokenManager.GetTokenAsync(realmId);
                if (string.IsNullOrWhiteSpace(token)) return false;

                var context = new ServiceContext(realmId, IntuitServicesType.QBO, new OAuth2RequestValidator(token));
                var companyInfoService = new DataService(context);
                var info = companyInfoService.FindAll(new CompanyInfo()).FirstOrDefault();
                return info != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "QuickBooks connection verification failed for realm {RealmId}", realmId);
                return false;
            }
        }
    }
}