using PropertyManagementAPI.Domain.DTOs.Vendors;

namespace PropertyManagementAPI.Application.Services.Vendors
{
    public interface IVendorService
    {
        Task<IEnumerable<VendorDto>> GetAllVendorsAsync();
        Task<VendorDto> GetVendorByIdAsync(int vendorId);
        Task<VendorDto> CreateVendorAsync(VendorDto vendorDto);
        Task<bool> UpdateVendorAsync(int vendorId, VendorDto vendorDto);
        Task<bool> SetIsActiveAsync(int vendorId);
    }
}
