using PropertyManagementAPI.Domain.Entities.Property;
using PropertyManagementAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs.Property;

namespace PropertyManagementAPI.Infrastructure.Repositories.Property
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
            var property = new Properties
            {
                PropertyName = dto.PropertyName,
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

            _context.Properties.Add(property);
            await _context.SaveChangesAsync();

            var propertyOwner = new PropertyOwner
            {
                PropertyId = property.PropertyId,
                OwnerId = dto.OwnerId,
                OwnershipPercentage = dto.OwnershipPercentage
            };

            _context.PropertyOwners.Add(propertyOwner);
            await _context.SaveChangesAsync();

            _context.PropertyOwners.Add(propertyOwner);
            await _context.SaveChangesAsync();

            dto.PropertyId = property.PropertyId;
            return dto;
        }

        public async Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync()
        {
            return await _context.Properties
                .Select(p => new PropertyDto
                {
                    PropertyId = p.PropertyId,
                    PropertyName = p.PropertyName,
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

        public async Task<PropertyAddressDto?> GetPropertyByIdAsync(int propertyId)
        {
            var result = await _context.Properties
                .Where(p => p.PropertyId == propertyId)
                .Select(p => new PropertyAddressDto // Change PropertyAddressDto to PropertyDto
                {
                    PropertyId = p.PropertyId, // Add PropertyId mapping
                    PropertyName = p.PropertyName,
                    Address = p.Address,
                    Address1 = p.Address1,
                    City = p.City,
                    State = p.State,
                    PostalCode = p.PostalCode,
                    Country = p.Country,
                    Amount = _context.Pricing
                        .Where(pr => pr.PropertyId == p.PropertyId)
                        .OrderByDescending(pr => pr.EffectiveDate)
                        .Select(pr => pr.RentalAmount)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            return result;
        }

        public async Task<IEnumerable<PropertyDto>> GetPropertiesByOwnerIdAsync(int ownerId)
        {
            return await _context.PropertyOwners
                .Where(po => po.OwnerId == ownerId)
                .Include(po => po.Properties)
                .Select(po => new PropertyDto
                {
                    PropertyId = po.Properties.PropertyId,
                    PropertyName = po.Properties.PropertyName,
                    Address = po.Properties.Address,
                    Address1 = po.Properties.Address1,
                    City = po.Properties.City,
                    State = po.Properties.State,
                    PostalCode = po.Properties.PostalCode,
                    Country = po.Properties.Country,
                    Bedrooms = po.Properties.Bedrooms,
                    Bathrooms = po.Properties.Bathrooms,
                    SquareFeet = po.Properties.SquareFeet,
                    PropertyTaxes = po.Properties.PropertyTaxes,
                    IsAvailable = po.Properties.IsAvailable,
                    IsActive = po.Properties.IsActive,
                    OwnerId = po.OwnerId,
                    OwnershipPercentage = po.OwnershipPercentage
                })
                .ToListAsync();
        }

        public async Task<PropertyDto?> UpdatePropertyAsync(PropertyDto dto)
        {
            var property = await _context.Properties.FindAsync(dto.PropertyId);
            if (property == null) return null;

            property.PropertyName = dto.PropertyName;
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
            var property = await _context.Properties.FindAsync(propertyId);
            if (property == null) return false;

            property.IsActive = !property.IsActive;

            var save = await _context.SaveChangesAsync();
            return save > 0;
        }
    }
}
