using PropertyManagementAPI.Domain.Entities;

namespace PropertyManagementAPI.Infrastructure.Repositories.Vendors
{
    public interface IVendorRepository
    {
        Task<IEnumerable<Vendor>> GetAllAsync();
        Task<Vendor> GetByIdAsync(int vendorId);
        Task<Vendor> CreateAsync(Vendor vendor);
        Task<bool> UpdateAsync(Vendor vendor);
        Task<bool> SetIsActiveAsync(int vendorId);
    }
}
