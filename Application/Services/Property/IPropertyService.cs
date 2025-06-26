using PropertyManagementAPI.Domain.DTOs;

namespace PropertyManagementAPI.Application.Services.Property
{
    public interface IPropertyService
    {
        /// <summary>
        /// Adds a new property and links it to an owner via PropertyOwners.
        /// </summary>
        Task<PropertyDto> AddPropertyAsync(PropertyDto propertyDto);

        /// <summary>
        /// Retrieves all properties in the system.
        /// </summary>
        Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync();

        /// <summary>
        /// Retrieves a property by its unique ID.
        /// </summary>
        Task<PropertyDto?> GetPropertyByIdAsync(int propertyId);

        /// <summary>
        /// Retrieves all properties owned by a specific owner.
        /// </summary>
        Task<IEnumerable<PropertyDto>> GetPropertiesByOwnerIdAsync(int ownerId);

        /// <summary>
        /// Updates an existing property.
        /// </summary>
        Task<PropertyDto?> UpdatePropertyAsync(int propertyId, PropertyDto propertyDto);

        /// <summary>
        /// Soft-deletes or deactivates a property.
        /// </summary>
        Task<bool> SetActivatePropertyAsync(int propertyId);
    }
}