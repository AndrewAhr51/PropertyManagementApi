using PropertyManagementAPI.Domain.DTOs.Property;
using PropertyManagementAPI.Infrastructure.Repositories.Property;

namespace PropertyManagementAPI.Application.Services.Property
{
    public class PropertyPhotosService : IPropertyPhotosService
    {
        private readonly IPropertyPhotosRepository _repository;

        public PropertyPhotosService(IPropertyPhotosRepository repository)
        {
            _repository = repository;
        }

        public async Task<PropertyPhotosDto> UploadPhotoAsync(PropertyPhotosDto photoDto)
        {
            return await _repository.AddPhotoAsync(photoDto);
        }

        public async Task<IEnumerable<PropertyPhotosDto>> GetPhotosForPropertyAsync(int propertyId)
        {
            return await _repository.GetPhotosByPropertyIdAsync(propertyId);
        }

        public async Task<bool> RemovePhotoAsync(int photoId)
        {
            return await _repository.DeletePhotoAsync(photoId);
        }
    }
}
