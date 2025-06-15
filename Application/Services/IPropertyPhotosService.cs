using PropertyManagementAPI.Domain.DTOs;

namespace PropertyManagementAPI.Application.Services
{
    public interface IPropertyPhotosService
    {
        Task<PropertyPhotosDto> UploadPhotoAsync(PropertyPhotosDto photoDto);
        Task<IEnumerable<PropertyPhotosDto>> GetPhotosForPropertyAsync(int propertyId);
        Task<bool> RemovePhotoAsync(int photoId);
    }
}
