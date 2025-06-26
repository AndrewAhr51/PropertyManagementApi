using PropertyManagementAPI.Domain.Entities.Payments;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments
{
    public interface IPaymentRepository
    {
        Task<Payment> CreatePaymentAsync(Payment payment);
        Task<Payment?> GetPaymentByIdAsync(int paymentId);
        Task<IEnumerable<Payment>> GetPaymentByTenantIdAsync(int tenantId);
        Task SavePaymentChangesAsync();
    }

}
