using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.DTOs.Payments.Stripe;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments
{
    public interface IStripeRepository
    {
        Task<bool> CreateStripePaymentAsync(CreateStripeDto dto);
        Task<StripePaymentResponseDto> CreateStripePaymentIntentAsync(CreateStripeDto dto);
        Task<bool> ProcessStripePaymentAsync(CreateStripeDto dto);
    }
}
