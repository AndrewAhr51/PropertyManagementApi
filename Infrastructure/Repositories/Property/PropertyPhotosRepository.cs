using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Data;

namespace PropertyManagementAPI.Infrastructure.Repositories.Property
{
    public class PropertyPhotosRepository : IPropertyPhotosRepository
    {
        private readonly MySqlDbContext _context;

        public PropertyPhotosRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<PropertyPhotosDto> AddPhotoAsync(PropertyPhotosDto dto)
        {
            var entity = new PropertyPhotos
            {
                PropertyId = dto.PropertyId,
                PhotoUrl = dto.PhotoUrl,
                Room = dto.Room,
                Caption = dto.Caption,
                CreatedDate = dto.CreatedDate ?? DateTime.UtcNow
            };

            _context.PropertyPhotos.Add(entity);
            await _context.SaveChangesAsync();

            dto.PhotoId = entity.PhotoId;
            dto.CreatedDate = entity.CreatedDate;
            return dto;
        }

        public async Task<IEnumerable<PropertyPhotosDto>> GetPhotosByPropertyIdAsync(int propertyId)
        {
            return await _context.PropertyPhotos
                .Where(p => p.PropertyId == propertyId)
                .Select(p => new PropertyPhotosDto
                {
                    PhotoId = p.PhotoId,
                    PropertyId = p.PropertyId,
                    PhotoUrl = p.PhotoUrl,
                    Room = p.Room,
                    Caption = p.Caption,
                    CreatedDate = p.CreatedDate
                })
                .ToListAsync();
        }

        public async Task<bool> DeletePhotoAsync(int photoId)
        {
            var photo = await _context.PropertyPhotos.FindAsync(photoId);
            if (photo == null) return false;

            _context.PropertyPhotos.Remove(photo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
