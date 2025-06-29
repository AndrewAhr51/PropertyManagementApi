﻿using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs.Users;
using PropertyManagementAPI.Domain.Entities.User;
using PropertyManagementAPI.Infrastructure.Repositories.Owners;

namespace PropertyManagementAPI.Application.Services.Owners
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

        public async Task<bool> UpdateOwnerAsync( OwnerDto ownerDto)
        {
            var save = await _ownerRepository.UpdateOwnerAsync(ownerDto);
            return save;
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
