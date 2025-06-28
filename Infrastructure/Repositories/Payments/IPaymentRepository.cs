using PropertyManagementAPI.Domain.Entities.Payments;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments
{
    public interface IPaymentRepository
    {
        Task<Payment> GetByIdAsync(int paymentId);
        Task<IEnumerable<Payment>> GetByInvoiceIdAsync(int invoiceId);
        Task AddAsync(Payment payment);
        Task SaveChangesAsync();
    }

}
