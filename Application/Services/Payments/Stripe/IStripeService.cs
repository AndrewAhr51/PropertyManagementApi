using PropertyManagementAPI.Domain.DTOs.Payments.Stripe;
using Stripe;

namespace PropertyManagementAPI.Application.Services.Payments.Stripe
{
    public interface IStripeService
    {
        Task<PaymentIntent> ProcessPaymentIntentAsync(decimal amount, string currency);

        Task<StripePaymentResponseDto> CreateStripePaymentIntentAsync(CreateStripeDto dto);

        Task<StripePaymentResponseDto> ProcessStripePaymentAsync(CreateStripeDto dto);

        Task<StripeSessionDto?> GetSessionAsync(string sessionId);

    }
}
