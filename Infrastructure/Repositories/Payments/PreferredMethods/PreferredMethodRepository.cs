using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities.Payments.PreferredMethods;
using PropertyManagementAPI.Infrastructure.Data;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments.PreferredMethods
{
    public class PreferredMethodRepository : IPreferredMethodRepository
    {
        private readonly MySqlDbContext _context;

        public PreferredMethodRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<PreferredMethod> GetDefaultByTenantAsync(int tenantId)
        {
            var tenantExists = await _context.Tenants.AnyAsync(t => t.TenantId == tenantId);
            if (!tenantExists)
            {
                throw new KeyNotFoundException($"Tenant with ID {tenantId} not found.");
            }
            
            var peferredMethod =  await _context.PreferredMethods
                .AsNoTracking()
                .FirstOrDefaultAsync(pm => pm.TenantId == tenantId && pm.IsDefault);

            return peferredMethod ?? throw new KeyNotFoundException($"Preferred method for tenant ID {tenantId} not found.");
        }

        public async Task<PreferredMethod> GetDefaultByOwnerAsync(int ownerId)
        {
            var ownerExists = await _context.Owners.AnyAsync(o => o.OwnerId == ownerId);
            if (!ownerExists)
            {
                throw new KeyNotFoundException($"Owner with ID {ownerId} not found.");
            }
            // Fetch the default preferred method for the owner

            var peferredMethod = await _context.PreferredMethods
                .AsNoTracking()
                .FirstOrDefaultAsync(pm => pm.OwnerId == ownerId && pm.IsDefault);

            return peferredMethod ?? throw new KeyNotFoundException($"Preferred method for owner ID {ownerId} not found.");
        }

        public async Task<int> UpsertAsync(PreferredMethod method)
        {
            var existing = await _context.PreferredMethods
                .FirstOrDefaultAsync(pm =>
                    pm.TenantId == method.TenantId &&
                    pm.OwnerId == method.OwnerId &&
                    pm.MethodType == method.MethodType);

            if (existing != null)
            {
                existing.CardTokenId = method.CardTokenId;
                existing.BankAccountInfoId = method.BankAccountInfoId;
                existing.IsDefault = method.IsDefault;
                existing.UpdatedOn = DateTime.UtcNow;
            }
            else
            {
                await _context.PreferredMethods.AddAsync(method);
            }

            await _context.SaveChangesAsync();
            return method.PreferredMethodId;
        }

        public async Task ClearDefaultAsync(int? tenantId, int? ownerId)
        {
            var query = _context.PreferredMethods.Where(pm => pm.IsDefault);

            if (tenantId.HasValue)
                query = query.Where(pm => pm.TenantId == tenantId.Value);

            if (ownerId.HasValue)
                query = query.Where(pm => pm.OwnerId == ownerId.Value);

            var existingDefaults = await query.ToListAsync();

            foreach (var method in existingDefaults)
            {
                method.IsDefault = false;
                method.UpdatedOn = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }
}
