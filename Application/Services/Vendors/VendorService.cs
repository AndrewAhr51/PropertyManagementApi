using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Repositories.Vendors;
using System.Numerics;

namespace PropertyManagementAPI.Application.Services.Vendors
{
    public class VendorService : IVendorService
    {
        private readonly IVendorRepository _vendorRepository;

        public VendorService(IVendorRepository vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }

        public async Task<IEnumerable<VendorDto>> GetAllVendorsAsync()
        {
            var vendors = await _vendorRepository.GetAllAsync();
            return vendors.Select(v => new VendorDto
            {
                VendorId = v.VendorId,
                Name = v.Name,
                ServiceTypeId = v.ServiceTypeId,
                ContactEmail = v.ContactEmail,
                ContactFirstName = v.ContactFirstName,
                ContactLastName = v.ContactLastName,
                PhoneNumber = v.PhoneNumber,
                Address = v.Address,
                Address1 = v.Address1,
                City = v.City,
                State = v.State,
                PostalCode = v.PostalCode,
                AccountNumber = v.AccountNumber,
                Notes = v.Notes,
            }).ToList();
        }

        public async Task<VendorDto> GetVendorByIdAsync(int vendorId)
        {
            var vendor = await _vendorRepository.GetByIdAsync(vendorId);
            if (vendor == null) return null;

            return new VendorDto
            {
                VendorId = vendor.VendorId,
                Name = vendor.Name,
                ServiceTypeId = vendor.ServiceTypeId,
                ContactEmail = vendor.ContactEmail,
                ContactFirstName = vendor.ContactFirstName,
                ContactLastName = vendor.ContactLastName,
                PhoneNumber = vendor.PhoneNumber,
                Address = vendor.Address,
                Address1 = vendor.Address1,
                City = vendor.City,
                State = vendor.State,
                PostalCode = vendor.PostalCode,
                AccountNumber = vendor.AccountNumber,
                Notes = vendor.Notes,
            };
        }

        public async Task<VendorDto> CreateVendorAsync(VendorDto vendorDto)
        {
            var vendor = new Vendor
            {
                Name = vendorDto.Name,
                ServiceTypeId = vendorDto.ServiceTypeId,
                ContactEmail = vendorDto.ContactEmail,
                ContactFirstName = vendorDto.ContactFirstName,
                ContactLastName = vendorDto.ContactLastName,
                PhoneNumber = vendorDto.PhoneNumber,
                Address = vendorDto.Address,
                Address1 = vendorDto.Address1,
                City = vendorDto.City,
                State = vendorDto.State,
                PostalCode = vendorDto.PostalCode,
                AccountNumber = vendorDto.AccountNumber,
                Notes = vendorDto.Notes,
                IsActive = true,
            };

            var createdVendor = await _vendorRepository.CreateAsync(vendor);

            return new VendorDto
            {
                VendorId = createdVendor.VendorId,
                Name = createdVendor.Name,
                ServiceTypeId = createdVendor.ServiceTypeId,
                ContactEmail = createdVendor.ContactEmail,
                ContactFirstName = vendorDto.ContactFirstName,
                ContactLastName = vendorDto.ContactLastName,
                PhoneNumber = createdVendor.PhoneNumber,
                Address = createdVendor.Address,
                Address1 = createdVendor.Address1,
                City = createdVendor.City,
                State = createdVendor.State,
                PostalCode = createdVendor.PostalCode,
                AccountNumber = createdVendor.AccountNumber,
                Notes = createdVendor.Notes,
            };
        }

        public async Task<bool> UpdateVendorAsync(int vendorId, VendorDto vendorDto)
        {
            var vendor = await _vendorRepository.GetByIdAsync(vendorId);
            if (vendor == null) return false;

            vendor.Name = vendorDto.Name;
            vendor.ServiceTypeId = vendorDto.ServiceTypeId;
            vendor.ContactEmail = vendorDto.ContactEmail;
            vendor.ContactFirstName = vendorDto.ContactFirstName;
            vendor.ContactLastName = vendorDto.ContactLastName;                
            vendor.PhoneNumber = vendorDto.PhoneNumber;
            vendor.Address = vendorDto.Address;
            vendor.Address1 = vendorDto.Address1;
            vendor.City = vendorDto.City;
            vendor.State = vendorDto.State;
            vendor.PostalCode = vendorDto.PostalCode;
            vendor.AccountNumber = vendorDto.AccountNumber;
            vendor.Notes = vendorDto.Notes;

            return await _vendorRepository.UpdateAsync(vendor);
        }

        public async Task<bool> SetIsActiveAsync(int vendorId)
        {
            return await _vendorRepository.SetIsActiveAsync(vendorId); 
        }
    }
}
