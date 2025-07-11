using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs.Documents;
using PropertyManagementAPI.Domain.Entities.Documents;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Repositories.Documents;

namespace PropertyManagementAPI.Infrastructure.Repositories.Documents
{

    public class DocumentReferenceRepository : IDocumentReferenceRepository
    {
        private readonly MySqlDbContext _context;

        public DocumentReferenceRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<DocumentReferenceDto?> GetDocumentReferenceByIdAsync(int referenceId)
        {
            var r = await _context.DocumentReferences.FindAsync(referenceId);
            return r == null ? null : MapToDto(r);
        }

        public async Task<IEnumerable<DocumentReferenceDto>> GetDocumentReferenceByDocumentIdAsync(int documentId)
        {
            return await _context.DocumentReferences
                .Where(r => r.DocumentId == documentId)
                .Select(r => MapToDto(r))
                .ToListAsync();
        }

        public async Task<IEnumerable<DocumentReferenceDto>> GetDocumentReferenceByEntityAsync(string type, int entityId)
        {
            return await _context.DocumentReferences
                .Where(r => r.RelatedEntityType == type && r.RelatedEntityId == entityId)
                .Select(r => MapToDto(r))
                .ToListAsync();
        }

        public async Task<IEnumerable<DocumentReferenceDto>> GetDocumentReferenceByCreatorAsync(int userId)
        {
            return await _context.DocumentReferences
                .Where(r => r.LinkedByUserId == userId)
                .Select(r => MapToDto(r))
                .ToListAsync();
        }

        public async Task<DocumentReferenceDto> AddDocumentReferenceAsync(DocumentReferenceDto dto)
        {
            var entity = new DocumentReference
            {
                DocumentId = dto.DocumentId,
                RelatedEntityType = dto.RelatedEntityType,
                RelatedEntityId = dto.RelatedEntityId,
                AccessRole = dto.AccessRole,
                LinkDate = dto.LinkDate,
                LinkedByUserId = dto.LinkedByUserId,
                Description = dto.Description
            };

            _context.DocumentReferences.Add(entity);
            await _context.SaveChangesAsync();

            dto.Id = entity.Id;
            return dto;
        }

        public async Task<bool> RemoveDocumentReferenceAsync(int referenceId)
        {
            var entity = await _context.DocumentReferences.FindAsync(referenceId);
            if (entity == null) return false;

            _context.DocumentReferences.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        private static DocumentReferenceDto MapToDto(DocumentReference r) => new DocumentReferenceDto
        {
            Id = r.Id,
            DocumentId = r.DocumentId,
            RelatedEntityType = r.RelatedEntityType,
            RelatedEntityId = r.RelatedEntityId,
            AccessRole = r.AccessRole,
            LinkDate = r.LinkDate,
            LinkedByUserId = r.LinkedByUserId,
            Description = r.Description
        };
    }
}