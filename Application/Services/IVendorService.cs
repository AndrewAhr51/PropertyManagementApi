using PropertyManagementAPI.Domain.DTOs;

namespace PropertyManagementAPI.Application.Services
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
