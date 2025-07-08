using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PropertyManagementAPI.Domain.DTOs.Payments.PayPal;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public class PayPalPaymentProcessor : IPayPalPaymentProcessor
    {
        private readonly PayPalHttpClient _client;
        private object CheckoutPaymentIntent;

        public PayPalPaymentProcessor(PayPalHttpClient client)
        {
            _client = client;
        }

        public async Task<PayPalOrderResult> CreatePayPalOrderAsync(decimal amount, string currency, Invoice invoice, string idempotencyKey)
        {
            var orderRequest = new OrdersCreateRequest();
            orderRequest.Prefer("return=representation");
            orderRequest.RequestBody(new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>
            {
                new PurchaseUnitRequest
                {
                    AmountWithBreakdown = new AmountWithBreakdown
                    {
                        CurrencyCode = currency,
                        Value = amount.ToString("F2")
                    },
                    Description = $"Invoice {invoice.ReferenceNumber}"
                }
            }
            });

            var response = await _client.Execute(orderRequest);
            var result = response.Result<Order>();

            var approvalLink = result.Links.FirstOrDefault(l => l.Rel == "approve")?.Href;

            return new PayPalOrderResult
            {
                OrderId = result.Id,
                ApprovalLink = approvalLink
            };
        }

        public async Task<string> CapturePayPalOrderAsync(string orderId)
        {
            var request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest());

            var response = await _client.Execute(request);
            var capturedOrder = response.Result<Order>();

            return capturedOrder.Status;
        }

        public async Task<string> GetPayPalApprovalLinkAsync(string orderId)
        {
            var request = new OrdersGetRequest(orderId);
            var response = await _client.Execute(request);
            var order = response.Result<Order>();

            return order.Links.FirstOrDefault(l => l.Rel == "approve")?.Href;
        }

        public Task<CardPayment> CapturePayPalCardPaymentAsync(string orderId, CreatePayPalDto dto, Invoice invoice)
        {
            throw new NotImplementedException();
        }
    }
}
