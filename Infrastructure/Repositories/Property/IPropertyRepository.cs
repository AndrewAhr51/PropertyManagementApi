﻿using PropertyManagementAPI.Domain.DTOs.Property;

namespace PropertyManagementAPI.Infrastructure.Repositories.Property
{
    public interface IPropertyRepository
    {
        Task<PropertyDto> AddPropertyAsync(PropertyDto propertyDto);
        Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync();
        Task<PropertyDto?> GetPropertyByIdAsync(int propertyId);
        Task<IEnumerable<PropertyDto>> GetPropertiesByOwnerIdAsync(int ownerId);
        Task<PropertyDto?> UpdatePropertyAsync(PropertyDto propertyDto);
        Task<bool> SetActivatePropertyAsync(int propertyId);
    }
}
