using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Infrastructure.Data;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments
{
    public class PaymentMethodsRepository : IPaymentMethodsRepository
    {
        private readonly MySqlDbContext _context;

        public PaymentMethodsRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentMethodsDto> AddAsync(PaymentMethodsDto dto)
        {
            var entity = new PaymentMethods
            {
                MethodName = dto.MethodName,
                Description = dto.Description,
                IsActive = dto.IsActive
            };

            _context.PaymentMethods.Add(entity);
            await _context.SaveChangesAsync();

            dto.PaymentMethodId = entity.PaymentMethodId;
            return dto;
        }

        public async Task<IEnumerable<PaymentMethodsDto>> GetAllAsync()
        {
            return await _context.PaymentMethods
                .Select(pm => new PaymentMethodsDto
                {
                    PaymentMethodId = pm.PaymentMethodId,
                    MethodName = pm.MethodName,
                    Description = pm.Description,
                    IsActive = pm.IsActive
                })
                .ToListAsync();
        }

        public async Task<PaymentMethodsDto?> GetByIdAsync(int id)
        {
            var pm = await _context.PaymentMethods.FindAsync(id);
            return pm == null ? null : new PaymentMethodsDto
            {
                PaymentMethodId = pm.PaymentMethodId,
                MethodName = pm.MethodName,
                Description = pm.Description,
                IsActive = pm.IsActive
            };
        }

        public async Task<bool> UpdateAsync(int id, PaymentMethodsDto dto)
        {
            var pm = await _context.PaymentMethods.FindAsync(id);
            if (pm == null) return false;

            pm.MethodName = dto.MethodName;
            pm.Description = dto.Description;
            pm.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var pm = await _context.PaymentMethods.FindAsync(id);
            if (pm == null) return false;

            _context.PaymentMethods.Remove(pm);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
