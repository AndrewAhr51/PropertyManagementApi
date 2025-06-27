using PropertyManagementAPI.Domain.DTOs.Payments;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public interface IPaymentMethodsService
    {
        Task<PaymentMethodsDto> CreateAsync(PaymentMethodsDto dto);
        Task<IEnumerable<PaymentMethodsDto>> GetAllAsync();
        Task<PaymentMethodsDto?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, PaymentMethodsDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
