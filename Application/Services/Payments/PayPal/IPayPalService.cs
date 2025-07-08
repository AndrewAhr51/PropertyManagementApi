using PropertyManagementAPI.Domain.DTOs.Invoices;
using PropertyManagementAPI.Domain.DTOs.Payments.PayPal;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;

namespace PropertyManagementAPI.Application.Services.Payments.PayPal
{
    public interface IPayPalService
    {
        Task<PayPalPaymentResponseDto> InitializePayPalOrderAsync(CreatePayPalDto dto);
        Task<CardPayment> CapturePayPalCardPaymentAsync(string orderId, CreatePayPalDto dto, Invoice invoice);
        Task<string> GetPayPalApprovalLinkAsync(string orderId);
        Task<PayPalCaptureResponseDto> CaptureOrderAsync(string orderId);
    }
}
