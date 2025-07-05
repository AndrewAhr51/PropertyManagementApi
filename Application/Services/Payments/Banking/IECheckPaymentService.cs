using PropertyManagementAPI.Domain.Entities.Payments.Banking;

namespace PropertyManagementAPI.Application.Services.Payments.Banking
{
    public interface IECheckPaymentService
    {
        Task<bool> AddBankAccountAsync(int tenantId, string bankName, string last4, string stripeBankAccountId);
        Task<PaymentTransactions> SubmitPaymentAsync(int tenantId, decimal amount, string stripePaymentIntentId);
        Task<IEnumerable<PaymentTransactions>> GetTenantTransactionsAsync(int tenantId);
    }

}
