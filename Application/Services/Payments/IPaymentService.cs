using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.DTOs.Payments.PayPal;
using PropertyManagementAPI.Domain.DTOs.Payments.Stripe;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public interface IPaymentService
    {
        // Standard payments
        Task<Payment> CreatePaymentAsync(CreatePaymentDto dto);

        Task<bool> ReversePaymentAsync(int paymentId);

        Task<Payment> GetPaymentByIdAsync(int paymentId);
        Task<IEnumerable<Payment>> GetPaymentsByInvoiceIdAsync(int invoiceId);

        // PayPal flow
        Task<PayPalPaymentResponseDto> CreatePayPalOrderAsync(CreatePayPalDto dto);
        Task<CardPayment> CapturePayPalCardPaymentAsync(string orderId, CreatePayPalDto dto, Invoice invoice);

        // Invoice lookup
        Task<Invoice?> GetInvoiceAsync(int invoiceId);
        Task<string> CreatePayPalOrderAsync(decimal amount, string currency, Invoice invoice);
        Task<string> GetApprovalLinkAsync(string orderId);
        Task <StripePaymentResponseDto>CreateStripePaymentIntentAsync(CreateStripeDto dto);
    }
}