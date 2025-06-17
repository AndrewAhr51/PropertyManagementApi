using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Data;

namespace PropertyManagementAPI.Infrastructure.Repositories
{
    public class VendorRepository : IVendorRepository
    {
        private readonly SQLServerDbContext _context;

        public VendorRepository(SQLServerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vendor>> GetAllAsync()
        {
            return await _context.Vendors.ToListAsync();
        }

        public async Task<Vendor> GetByIdAsync(int vendorId)
        {
            return await _context.Vendors.FindAsync(vendorId);
        }

        public async Task<Vendor> CreateAsync(Vendor vendor)
        {
            // Check if a vendor with the same name already exists
            var existingVendor = await _context.Vendors
                .FirstOrDefaultAsync(v => v.Name == vendor.Name);

            if (existingVendor != null)
            {
                throw new InvalidOperationException($"A vendor with the name '{vendor.Name}' already exists.");
            }

            _context.Vendors.Add(vendor);
            await _context.SaveChangesAsync();
            return vendor;
        }

        public async Task<bool> UpdateAsync(Vendor vendor)
        {
            _context.Vendors.Update(vendor);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SetIsActiveAsync(int vendorId)
        {
            var vendor = await _context.Vendors.FindAsync(vendorId);
            if (vendor == null) return false;

            vendor.IsActive = !vendor.IsActive; 
            _context.Vendors.Update(vendor);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
