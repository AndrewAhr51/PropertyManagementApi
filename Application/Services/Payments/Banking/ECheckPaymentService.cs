using PropertyManagementAPI.Domain.Entities.Payments.Banking;
using PropertyManagementAPI.Infrastructure.Repositories.Payments.Banking;

namespace PropertyManagementAPI.Application.Services.Payments.Banking
{
    public class ECheckPaymentService : IECheckPaymentService
    {
        private readonly IBankAccountRepository _bankRepo;
        private readonly IPaymentTransactionRepository _txnRepo;

        public ECheckPaymentService(
            IBankAccountRepository bankRepo,
            IPaymentTransactionRepository txnRepo)
        {
            _bankRepo = bankRepo;
            _txnRepo = txnRepo;
        }

        public async Task<BankAccount> AddBankAccountAsync(int tenantId, string bankName, string last4, string stripeBankAccountId)
        {
            var account = new BankAccount
            {
                TenantId = tenantId,
                BankName = bankName,
                Last4 = last4,
                StripeBankAccountId = stripeBankAccountId,
                AccountType = "checking",
                IsVerified = true
            };

            return await _bankRepo.AddAsync(account);
        }

        public async Task<PaymentTransactions> SubmitPaymentAsync(int tenantId, decimal amount, string stripePaymentIntentId)
        {
            var txn = new PaymentTransactions
            {
                TenantId = tenantId,
                Amount = amount,
                StripePaymentIntentId = stripePaymentIntentId,
                Status = "pending"
            };

            return await _txnRepo.LogAsync(txn);
        }

        public async Task<IEnumerable<PaymentTransactions>> GetTenantTransactionsAsync(int tenantId)
        {
            return await _txnRepo.GetByTenantIdAsync(tenantId);
        }
    }
}
