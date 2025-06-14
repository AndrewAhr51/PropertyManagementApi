using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Repositories;

namespace PropertyManagementAPI.Application.Services
{
    public class OwnersService : IOwnersService
    {
        private readonly IOwnerRepository _ownerRepository;

        public OwnersService(IOwnerRepository ownerRepository)
        {
            _ownerRepository = ownerRepository;
        }

        public async Task<Owner> AddOwnerAsync(OwnerDto ownerDto)
        {
            return await _ownerRepository.AddOwnerAsync(ownerDto);
        }

        public async Task<IEnumerable<Owner>> GetAllOwnersAsync()
        {
            return await _ownerRepository.GetAllOwnersAsync();
        }

        public async Task<OwnerDto?> GetOwnerByIdAsync(int ownerId)
        {
            var owner = await _ownerRepository.GetOwnerByIdAsync(ownerId);
            return owner == null ? null : MapToOwnerDto(owner);
        }

        public async Task<OwnerDto?> GetOwnerByUserNameAsync(string username)
        {
            var owner = await _ownerRepository.GetOwnerByUserNameAsync(username);
            return owner == null ? null : MapToOwnerDto(owner);
        }

        public async Task<OwnerDto?> UpdateOwnerAsync(int ownerId, OwnerDto ownerDto)
        {
            var updatedOwner = await _ownerRepository.UpdateOwnerAsync(ownerId, ownerDto);
            return updatedOwner == null ? null : MapToOwnerDto(updatedOwner);
        }

        public async Task<bool> DeleteOwnerAsync(int ownerId)
        {
            return await _ownerRepository.DeleteOwnerAsync(ownerId);
        }

        public async Task<bool> SetActivateOwnerAsync(int ownerId)
        {
            return await _ownerRepository.SetActivateOwnerAsync(ownerId);
        }

        private OwnerDto MapToOwnerDto(Owner owner)
        {
            return new OwnerDto
            {
                OwnerId = owner.OwnerId,
                FirstName = owner.FirstName,
                LastName = owner.LastName,
                Email = owner.Email,
                Phone = owner.Phone,
                Address1 = owner.Address1,
                Address2 = owner.Address2,
                City = owner.City,
                State = owner.State,
                PostalCode = owner.PostalCode,
                Country = owner.Country
            };
        }
    }
}
