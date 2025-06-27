using PropertyManagementAPI.Domain.DTOs.Property;

namespace PropertyManagementAPI.Infrastructure.Repositories.Property
{
    public interface IPropertyPhotosRepository
    {
        Task<PropertyPhotosDto> AddPhotoAsync(PropertyPhotosDto photoDto);
        Task<IEnumerable<PropertyPhotosDto>> GetPhotosByPropertyIdAsync(int propertyId);
        Task<bool> DeletePhotoAsync(int photoId);
    }
}
