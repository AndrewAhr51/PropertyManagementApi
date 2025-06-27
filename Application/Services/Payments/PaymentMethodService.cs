using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Infrastructure.Repositories.Payments;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public class PaymentMethodsService : IPaymentMethodsService
    {
        private readonly IPaymentMethodsRepository _repository;

        public PaymentMethodsService(IPaymentMethodsRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaymentMethodsDto> CreateAsync(PaymentMethodsDto dto)
        {
            return await _repository.AddAsync(dto);
        }

        public async Task<IEnumerable<PaymentMethodsDto>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<PaymentMethodsDto?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<bool> UpdateAsync(int id, PaymentMethodsDto dto)
        {
            return await _repository.UpdateAsync(id, dto);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
