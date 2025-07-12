using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Domain.DTOs.Documents;
using PropertyManagementAPI.Domain.Entities.Documents;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Repositories.Documents;

namespace PropertyManagementAPI.Infrastructure.Repositories.Documents
{
    public class DocumentReferenceRepository : IDocumentReferenceRepository
    {
        private readonly MySqlDbContext _context;
        private readonly ILogger<DocumentReferenceRepository> _logger;

        public DocumentReferenceRepository(MySqlDbContext context, ILogger<DocumentReferenceRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DocumentReferenceDto?> GetDocumentReferenceByIdAsync(int referenceId)
        {
            try
            {
                var r = await _context.DocumentReferences.FindAsync(referenceId);
                return r == null ? null : MapToDto(r);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get reference by ID: {ReferenceId}", referenceId);
                return null;
            }
        }

        public async Task<IEnumerable<DocumentReferenceDto>> GetDocumentReferenceByDocumentIdAsync(int documentId)
        {
            try
            {
                return await _context.DocumentReferences
                    .Where(r => r.DocumentId == documentId)
                    .Select(r => MapToDto(r))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get references by DocumentId: {DocumentId}", documentId);
                return Enumerable.Empty<DocumentReferenceDto>();
            }
        }

        public async Task<IEnumerable<DocumentReferenceDto>> GetDocumentReferenceByEntityAsync(string type, int entityId)
        {
            try
            {
                return await _context.DocumentReferences
                    .Where(r => r.RelatedEntityType == type && r.RelatedEntityId == entityId)
                    .Select(r => MapToDto(r))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get references by entity: {EntityType} {EntityId}", type, entityId);
                return Enumerable.Empty<DocumentReferenceDto>();
            }
        }

        public async Task<IEnumerable<DocumentReferenceDto>> GetDocumentReferenceByCreatorAsync(int userId)
        {
            try
            {
                return await _context.DocumentReferences
                    .Where(r => r.RelatedEntityId == userId)
                    .Select(r => MapToDto(r))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get references by creator: {UserId}", userId);
                return Enumerable.Empty<DocumentReferenceDto>();
            }
        }

        public async Task<bool> AddDocumentReferenceAsync(DocumentReferenceDto dto)
        {
            try
            {
                var entity = new DocumentReference
                {
                    DocumentId = dto.DocumentId,
                    RelatedEntityType = dto.RelatedEntityType,
                    RelatedEntityId = dto.RelatedEntityId,
                    AccessRole = dto.AccessRole,
                    LinkedDate = dto.LinkedDate,
                    Description = dto.Description
                };

                _context.DocumentReferences.Add(entity);
                var save = await _context.SaveChangesAsync();

                dto.Id = entity.Id;
                _logger.LogInformation("Document reference added: {ReferenceId} for DocumentId: {DocumentId}", entity.Id, entity.DocumentId);
                return save > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add reference for DocumentId: {DocumentId}", dto.DocumentId);
                return false;
            }
        }

        public async Task<bool> RemoveDocumentReferenceAsync(int referenceId)
        {
            try
            {
                var entity = await _context.DocumentReferences.FindAsync(referenceId);
                if (entity == null)
                {
                    _logger.LogWarning("Reference not found for deletion: {ReferenceId}", referenceId);
                    return false;
                }

                _context.DocumentReferences.Remove(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Document reference removed: {ReferenceId}", referenceId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove reference: {ReferenceId}", referenceId);
                return false;
            }
        }

        private static DocumentReferenceDto MapToDto(DocumentReference r) => new DocumentReferenceDto
        {
            Id = r.Id,
            DocumentId = r.DocumentId,
            RelatedEntityType = r.RelatedEntityType,
            RelatedEntityId = r.RelatedEntityId,
            AccessRole = r.AccessRole,
            LinkedDate = r.LinkedDate,
            Description = r.Description
        };
    }
}