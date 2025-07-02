using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Payments;

namespace PropertyManagementAPI.Application.Services.Payments.PayPal
{
    public class PayPalPaymentProcessor : IPaymentProcessor
    {
        private readonly PayPalHttpClient _client;
        private readonly PaymentAuditLogger _auditLogger;

        public PayPalPaymentProcessor(PayPalClient client, PaymentAuditLogger auditLogger)
        {
            _client = client.Client();
            _auditLogger = auditLogger;
        }

        public async Task<string> CreatePayPalOrderAsync(decimal amount, string currency, Invoice invoice)
        {
            try
            {
                var orderRequest = new OrderRequest
                {
                    CheckoutPaymentIntent = "CAPTURE",
                    PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        ReferenceId = $"INV-{invoice.InvoiceId}",
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = currency,
                            Value = amount.ToString("F2")
                        },
                        Description = $"Payment for Invoice #{invoice.ReferenceNumber}"
                    }
                }
                };

                var request = new OrdersCreateRequest();
                request.Prefer("return=representation");
                request.RequestBody(orderRequest);

                var response = await _client.Execute(request);
                var result = response.Result<Order>();

                await _auditLogger.LogAsync(
                    action: "CreateOrder",
                    status: "SUCCESS",
                    response: new { OrderId = result.Id, Amount = amount, Currency = currency },
                    performedBy: "System"
                );

                return result.Id;
            }
            catch (Exception ex)
            {
                await _auditLogger.LogAsync(
                    action: "CreateOrder",
                    status: "FAILED",
                    response: new { Error = ex.Message, Amount = amount, Currency = currency, invoice.InvoiceId },
                    performedBy: "System"
                );

                throw;
            }
        }

        public async Task<string> GetApprovalLinkAsync(string orderId)
        {
            try
            {
                var request = new OrdersGetRequest(orderId);
                var response = await _client.Execute(request);
                var result = response.Result<Order>();

                var approvalLink = result.Links.FirstOrDefault(l => l.Rel == "approve")?.Href;

                if (string.IsNullOrEmpty(approvalLink))
                    throw new InvalidOperationException("Approval link not found in PayPal response.");

                return approvalLink;
            }
            catch (Exception ex)
            {
                await _auditLogger.LogAsync(
                    action: "GetApprovalLink",
                    status: "FAILED",
                    response: new { OrderId = orderId, Error = ex.Message },
                    performedBy: "System"
                );

                throw;
            }
        }

        public async Task<string> CapturePayPalOrderAsync(string orderId)
        {
            try
            {
                var request = new OrdersCaptureRequest(orderId);
                request.RequestBody(new OrderActionRequest());

                var response = await _client.Execute(request);
                var result = response.Result<Order>();

                await _auditLogger.LogAsync(
                    action: "CaptureOrder",
                    status: result.Status,
                    response: new { OrderId = result.Id, result.Status },
                    performedBy: "Web"
                );

                return result.Status;
            }
            catch (Exception ex)
            {
                await _auditLogger.LogAsync(
                    action: "CaptureOrder",
                    status: "FAILED",
                    response: new { OrderId = orderId, Error = ex.Message },
                    performedBy: "Web"
                );

                throw;
            }
        }
    }
}
