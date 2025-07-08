using PropertyManagementAPI.Domain.DTOs.Payments.PayPal;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments
{
    public interface IPayPalRepository
    {
        Task<PayPalApprovalDto> CreatePayPalOrderAsync(CreatePayPalDto dto);
        Task<CardPayment> CapturePayPalCardPaymentAsync(CreatePayPalDto dto);
        Task<PayPalPaymentResponseDto> ProcessPayPalPaymentAsync(CreatePayPalDto dto);
    }
}
