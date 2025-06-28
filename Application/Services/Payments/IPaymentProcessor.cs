using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.Entities.Payments;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public interface IPaymentProcessor
    {
        Task<Payment> ProcessPaymentAsync(CreatePaymentDto dto);
    }

}
