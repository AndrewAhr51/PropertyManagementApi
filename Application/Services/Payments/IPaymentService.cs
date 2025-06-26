using PropertyManagementAPI.Domain.DTOs.Payments;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public interface IPaymentService
    {
        Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto);
        Task<PaymentDto?> GetPaymentAsync(int paymentId);
        Task<IEnumerable<PaymentDto>> GetPaymentsByTenantAsync(int tenantId);
    }

}
