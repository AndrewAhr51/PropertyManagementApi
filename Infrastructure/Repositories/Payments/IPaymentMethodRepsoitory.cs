using PropertyManagementAPI.Domain.DTOs.Payments;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments
{
    public interface IPaymentMethodsRepository
    {
        Task<PaymentMethodsDto> AddAsync(PaymentMethodsDto dto);
        Task<IEnumerable<PaymentMethodsDto>> GetAllAsync();
        Task<PaymentMethodsDto?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, PaymentMethodsDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
