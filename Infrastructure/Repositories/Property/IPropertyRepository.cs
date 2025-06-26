using PropertyManagementAPI.Domain.DTOs;

namespace PropertyManagementAPI.Infrastructure.Repositories.Property
{
    public interface IPropertyRepository
    {
        Task<PropertyDto> AddPropertyAsync(PropertyDto propertyDto);
        Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync();
        Task<PropertyDto?> GetPropertyByIdAsync(int propertyId);
        Task<IEnumerable<PropertyDto>> GetPropertiesByOwnerIdAsync(int ownerId);
        Task<PropertyDto?> UpdatePropertyAsync(int propertyId, PropertyDto propertyDto);
        Task<bool> SetActivatePropertyAsync(int propertyId);
    }
}
