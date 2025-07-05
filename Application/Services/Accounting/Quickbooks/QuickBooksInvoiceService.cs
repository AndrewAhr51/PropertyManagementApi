using DocumentFormat.OpenXml.Vml;
using Intuit.Ipp.Core;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Security;
using Microsoft.OpenApi.Models;
using PropertyManagementAPI.Domain.DTOs.Invoices;
using PropertyManagementAPI.Domain.Entities.Invoices;
using Stripe = Stripe.Invoice;
using Customer = Intuit.Ipp.Data.Customer;
using Invoice = Intuit.Ipp.Data.Invoice;
using Line = Intuit.Ipp.Data.Line;
using ReferenceType = Intuit.Ipp.Data.ReferenceType;

namespace PropertyManagementAPI.Application.Services.Accounting.Quickbooks
{
    public class QuickBooksInvoiceService
    {
        private readonly QuickBooksTokenManager _tokenManager;

        public QuickBooksInvoiceService(QuickBooksTokenManager tokenManager)
        {
            _tokenManager = tokenManager;
        }

        public async Task<Invoice> CreateInvoiceAsync(string realmId, Customer customer, decimal amount, string itemId)
        {
            var token = await _tokenManager.GetTokenAsync();
            var oauthValidator = new OAuth2RequestValidator(token);

            var serviceContext = new ServiceContext(realmId, IntuitServicesType.QBO, oauthValidator);
            var dataService = new DataService(serviceContext);

            var invoice = new Invoice
            {
                CustomerRef = new ReferenceType { Value = customer.Id },
                Line = new[]
                {
            new Line
            {
                Amount = amount,
                DetailType = LineDetailTypeEnum.SalesItemLineDetail,
                AnyIntuitObject = new SalesItemLineDetail
                {
                    ItemRef = new ReferenceType { Value = itemId }
                }
            }
        }
            };

            return dataService.Add(invoice) as Invoice;
        }

    }
}
