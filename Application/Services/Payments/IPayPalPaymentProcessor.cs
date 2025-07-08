using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.DTOs.Payments.PayPal;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public interface IPayPalPaymentProcessor
    {
        Task<CardPayment> CapturePayPalCardPaymentAsync(string orderId, CreatePayPalDto dto, Invoice invoice);
        Task<PayPalOrderResult> CreatePayPalOrderAsync(decimal amount, string currency, Invoice invoice, string idempotencyKey);
        Task<string> CapturePayPalOrderAsync(string orderId);
        Task<string> GetPayPalApprovalLinkAsync(string orderId);
    }

}
