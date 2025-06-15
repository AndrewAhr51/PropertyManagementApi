using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Data;

namespace PropertyManagementAPI.Infrastructure.Repositories
{
    public class PropertyPhotosRepository : IPropertyPhotosRepository
    {
        private readonly AppDbContext _context;

        public PropertyPhotosRepository(AppDbContext context)
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
                UploadedAt = dto.UploadedAt ?? DateTime.UtcNow
            };

            _context.PropertyPhotos.Add(entity);
            await _context.SaveChangesAsync();

            dto.PhotoId = entity.PhotoId;
            dto.UploadedAt = entity.UploadedAt;
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
                    UploadedAt = p.UploadedAt
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
