using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace PropertyManagementAPI.Infrastructure.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly MySqlDbContext _context;

        public PropertyRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<PropertyDto> AddPropertyAsync(PropertyDto dto)
        {
            var property = new Property
            {
                Name = dto.Name,
                Address = dto.Address,
                Address1 = dto.Address1,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                Bedrooms = dto.Bedrooms,
                Bathrooms = dto.Bathrooms,
                SquareFeet = dto.SquareFeet,
                PropertyTaxes = dto.PropertyTaxes,
                IsAvailable = dto.IsAvailable,
                IsActive = dto.IsActive
            };

            _context.Property.Add(property);
            await _context.SaveChangesAsync();

            var propertyOwner = new PropertyOwner
            {
                PropertyId = property.PropertyId,
                OwnerId = dto.OwnerId,
                OwnershipPercentage = dto.OwnershipPercentage
            };

            _context.PropertyOwners.Add(propertyOwner);
            await _context.SaveChangesAsync();

            dto.PropertyId = property.PropertyId;
            return dto;
        }

        public async Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync()
        {
            return await _context.Property
                .Select(p => new PropertyDto
                {
                    PropertyId = p.PropertyId,
                    Name = p.Name,
                    Address = p.Address,
                    Address1 = p.Address1,
                    City = p.City,
                    State = p.State,
                    PostalCode = p.PostalCode,
                    Country = p.Country,
                    Bedrooms = p.Bedrooms,
                    Bathrooms = p.Bathrooms,
                    SquareFeet = p.SquareFeet,
                    PropertyTaxes = p.PropertyTaxes,
                    IsAvailable = p.IsAvailable,
                    IsActive = p.IsActive
                })
                .ToListAsync();
        }

        public async Task<PropertyDto?> GetPropertyByIdAsync(int propertyId)
        {
            var p = await _context.Property.FindAsync(propertyId);
            return p == null ? null : new PropertyDto
            {
                PropertyId = p.PropertyId,
                Name = p.Name,
                Address = p.Address,
                Address1 = p.Address1,
                City = p.City,
                State = p.State,
                PostalCode = p.PostalCode,
                Country = p.Country,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                SquareFeet = p.SquareFeet,
                PropertyTaxes = p.PropertyTaxes,
                IsAvailable = p.IsAvailable,
                IsActive = p.IsActive
            };
        }

        public async Task<IEnumerable<PropertyDto>> GetPropertiesByOwnerIdAsync(int ownerId)
        {
            return await _context.PropertyOwners
                .Where(po => po.OwnerId == ownerId)
                .Include(po => po.Property)
                .Select(po => new PropertyDto
                {
                    PropertyId = po.Property.PropertyId,
                    Name = po.Property.Name,
                    Address = po.Property.Address,
                    Address1 = po.Property.Address1,
                    City = po.Property.City,
                    State = po.Property.State,
                    PostalCode = po.Property.PostalCode,
                    Country = po.Property.Country,
                    Bedrooms = po.Property.Bedrooms,
                    Bathrooms = po.Property.Bathrooms,
                    SquareFeet = po.Property.SquareFeet,
                    PropertyTaxes = po.Property.PropertyTaxes,
                    IsAvailable = po.Property.IsAvailable,
                    IsActive = po.Property.IsActive,
                    OwnerId = po.OwnerId,
                    OwnershipPercentage = po.OwnershipPercentage
                })
                .ToListAsync();
        }

        public async Task<PropertyDto?> UpdatePropertyAsync(int propertyId, PropertyDto dto)
        {
            var property = await _context.Property.FindAsync(propertyId);
            if (property == null) return null;

            property.Name = dto.Name;
            property.Address = dto.Address;
            property.Address1 = dto.Address1;
            property.City = dto.City;
            property.State = dto.State;
            property.PostalCode = dto.PostalCode;
            property.Country = dto.Country;
            property.Bedrooms = dto.Bedrooms;
            property.Bathrooms = dto.Bathrooms;
            property.SquareFeet = dto.SquareFeet;
            property.PropertyTaxes = dto.PropertyTaxes;
            property.IsAvailable = dto.IsAvailable;
            property.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();
            return dto;
        }

        public async Task<bool> SetActivatePropertyAsync(int propertyId)
        {
            var property = await _context.Property.FindAsync(propertyId);
            if (property == null) return false;

            property.IsActive = !property.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
