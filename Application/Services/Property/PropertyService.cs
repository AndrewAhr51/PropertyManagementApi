using PropertyManagementAPI.Domain.DTOs.Property;
using PropertyManagementAPI.Infrastructure.Repositories.Owners;
using PropertyManagementAPI.Infrastructure.Repositories.Property;

namespace PropertyManagementAPI.Application.Services.Property
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IOwnerRepository _ownerRepository;

        public PropertyService(IPropertyRepository propertyRepository, IOwnerRepository ownerRepository)
        {
            _propertyRepository = propertyRepository;
            _ownerRepository = ownerRepository;
        }

        public async Task<PropertyDto> AddPropertyAsync(PropertyDto propertyDto)
        {
            if (propertyDto == null)
                throw new ArgumentException("Property data cannot be null.");

            var owner = await _ownerRepository.GetOwnerByIdAsync(propertyDto.OwnerId);
            if (owner == null || !owner.IsActive)
                throw new InvalidOperationException($"Owner with ID {propertyDto.OwnerId} does not exist or is inactive.");

            var property = await _propertyRepository.AddPropertyAsync(propertyDto);
            return property;
        }

        public async Task<IEnumerable<PropertyDto>> GetAllPropertiesAsync()
        {
            return await _propertyRepository.GetAllPropertiesAsync();
        }

        public async Task<PropertyAddressDto?> GetPropertyByIdAsync(int propertyId)
        {
            return await _propertyRepository.GetPropertyByIdAsync(propertyId);
        }

        public async Task<IEnumerable<PropertyDto>> GetPropertiesByOwnerIdAsync(int ownerId)
        {
            return await _propertyRepository.GetPropertiesByOwnerIdAsync(ownerId);
        }

        public async Task<PropertyDto?> UpdatePropertyAsync(PropertyDto propertyDto)
        {
            return await _propertyRepository.UpdatePropertyAsync(propertyDto);
        }

        public async Task<bool> SetActivatePropertyAsync(int propertyId)
        {
            return await _propertyRepository.SetActivatePropertyAsync(propertyId);
        }
    }
}
