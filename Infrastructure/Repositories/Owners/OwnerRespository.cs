﻿using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs.Users;
using PropertyManagementAPI.Domain.Entities.User;
using PropertyManagementAPI.Infrastructure.Data;

namespace PropertyManagementAPI.Infrastructure.Repositories.Owners
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly MySqlDbContext _context;

        public OwnerRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<Owner> AddOwnerAsync(OwnerDto ownerDto)
        {
            // ✅ Check if UserId exists in the Users table
            var user = await _context.Users
                .Where(u => u.UserId == ownerDto.UserId)
                .FirstOrDefaultAsync();

            // ✅ Ensure the user has RoleId = 4 (Owner)
            if (user.RoleId != 4)
                throw new InvalidOperationException($"UserId {ownerDto.OwnerId} is not an owner. RoleId must be 4.");

            // ✅ Check if the owner already exists
            var existingOwner = await _context.Owners
                .Where(o => o.OwnerId == ownerDto.OwnerId)
                .FirstOrDefaultAsync();

            if (existingOwner != null)
                throw new InvalidOperationException($"Owner with UserId {ownerDto.OwnerId} already exists.");

            // ✅ Create new owner
            var owner = new Owner
            {
                UserId = ownerDto.UserId,
                FirstName = ownerDto.FirstName,
                LastName = ownerDto.LastName,
                Email = ownerDto.Email,
                Phone = ownerDto.Phone,
                Address1 = ownerDto.Address1,
                Address2 = ownerDto.Address2,
                City = ownerDto.City,
                State = ownerDto.State,
                PostalCode = ownerDto.PostalCode,
                Country = ownerDto.Country
            };

            try
            {
                _context.Owners.Add(owner);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the owner.", ex);
            }

            return owner;
        }

        public async Task<IEnumerable<Owner>> GetAllOwnersAsync()
        {
            return await _context.Owners
                .Include(o => o.Users) // ✅ Ensures User is loaded
                .ToListAsync();
        }

        public async Task<Owner?> GetOwnerByIdAsync(int ownerId)
        {
            return await _context.Owners
                .Include(o => o.Users) // ✅ Ensures User is loaded
                .FirstOrDefaultAsync(o => o.OwnerId == ownerId);
        }

        public async Task<Owner?> GetOwnerByUserNameAsync(string username)
        {
            return await _context.Owners
                .Include(o => o.Users) // ✅ Ensures User is loaded
                .Where(o => o.Users != null && o.Users.UserName == username && o.Users.RoleId == 4) // ✅ Prevents null reference issues
                .FirstOrDefaultAsync();
        }

        public async Task<bool> UpdateOwnerAsync(OwnerDto ownerDto)
        {
            if (ownerDto == null)
                throw new ArgumentException("Owner data cannot be null.");

            // ✅ Fetch the owner from the database
            var owner = await _context.Owners.FindAsync(ownerDto.OwnerId);
            if (owner == null)
                throw new KeyNotFoundException($"Owner with ID {ownerDto.OwnerId} not found.");

            // ✅ Validate that the email is unique (if changed)
            if (owner.Email != ownerDto.Email)
            {
                var emailExists = await _context.Owners.AnyAsync(o => o.Email == ownerDto.Email && o.OwnerId != ownerDto.OwnerId);
                if (emailExists)
                    throw new InvalidOperationException($"Email '{ownerDto.Email}' is already in use by another owner.");
            }

            // ✅ Update owner details
            owner.FirstName = ownerDto.FirstName;
            owner.LastName = ownerDto.LastName;
            owner.Email = ownerDto.Email;
            owner.Phone = ownerDto.Phone;
            owner.Address1 = ownerDto.Address1;
            owner.Address2 = ownerDto.Address2;
            owner.City = ownerDto.City;
            owner.State = ownerDto.State;
            owner.PostalCode = ownerDto.PostalCode;
            owner.Country = ownerDto.Country;
            owner.IsActive = ownerDto.IsActive;

           
            var save = await _context.SaveChangesAsync();
           
            return save > 0;
        }

        public async Task<bool> DeleteOwnerAsync(int ownerId)
        {
            var owner = await _context.Owners.FindAsync(ownerId);
            if (owner == null) return false;

            _context.Owners.Remove(owner);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetActivateOwnerAsync(int ownerId)
        {
            try
            {
                var owner = await _context.Owners.FindAsync(ownerId);
                if (owner == null) return false;

                // ✅ Toggle IsActive to the opposite value
                owner.IsActive = !owner.IsActive;

                var isActive = owner.IsActive;

                await _context.SaveChangesAsync();

                // ✅ Step 2: Get all PropertyIds owned by this owner
                var propertyIds = await _context.PropertyOwners
                    .Where(po => po.OwnerId == owner.OwnerId)
                    .Select(po => po.PropertyId)
                    .ToListAsync();

                // ✅ Step 3: Update IsActive on each property
                var properties = await _context.Properties
                    .Where(p => propertyIds.Contains(p.PropertyId))
                    .ToListAsync();

                foreach (var property in properties)
                {
                    property.IsActive = isActive;
                }

                await _context.SaveChangesAsync();


            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the owner's active status.", ex);
            }

            return true;
        }
    }
}
