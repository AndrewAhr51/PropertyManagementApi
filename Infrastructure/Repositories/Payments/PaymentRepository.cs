using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Infrastructure.Data;
using System;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments
{

    public class PaymentRepository : IPaymentRepository
    {
        private readonly MySqlDbContext _context;

        public PaymentRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> CreatePaymentAsync(Payment payment)
        {
            // Assign a unique reference number if not already set
            if (string.IsNullOrWhiteSpace(payment.ReferenceNumber))
            {
                payment.ReferenceNumber = ReferenceNumberHelper.Generate("PMT", payment.TenantId);
            }

            await _context.Payments.AddAsync(payment);
            return payment;
        }

        public Task<Payment?> GetPaymentByIdAsync(int paymentId)
        {
            return _context.Payments.FindAsync(paymentId).AsTask();
        }

        public Task<IEnumerable<Payment>> GetPaymentByTenantIdAsync(int tenantId)
        {
            return Task.FromResult<IEnumerable<Payment>>(
                _context.Payments.Where(p => p.TenantId == tenantId).ToList()
            );
        }

        public Task SavePaymentChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}
