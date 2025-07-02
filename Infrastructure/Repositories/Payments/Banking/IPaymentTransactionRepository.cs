using PropertyManagementAPI.Domain.Entities.Payments.Banking;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments.Banking
{
    public interface IPaymentTransactionRepository
    {
        Task<PaymentTransactions> LogAsync(PaymentTransactions txn);
        Task<IEnumerable<PaymentTransactions>> GetByTenantIdAsync(int tenantId);
    }
}
