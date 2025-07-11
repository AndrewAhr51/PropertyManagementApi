using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options; // Ensure this is included
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.DTOs.Documents;
using PropertyManagementAPI.Domain.Entities.Documents;
using PropertyManagementAPI.Infrastructure.Data;

namespace PropertyManagementAPI.Infrastructure.Repositories.Documents
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly MySqlDbContext _context;
        private readonly ILogger<DocumentRepository> _logger;
        private readonly EncryptionDocHelper _encryptionDocHelper;

        public int RetryDelay { get; private set; }
        public int MaxRetryCount { get; private set; }


        // Constructor of DocumentRepository
        public DocumentRepository(MySqlDbContext context, ILogger<DocumentRepository> logger, EncryptionDocHelper encryptionDocHelper)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DocumentDto> CreateDocumentAsync(DocumentDto dto)
        {
            try
            {
                var correlationId = string.IsNullOrWhiteSpace(dto.CorrelationId)
                    ? Guid.NewGuid().ToString()
                    : dto.CorrelationId;

                var entity = new Document
                {
                    Name = dto.Name,
                    MimeType = dto.MimeType,
                    SizeInBytes = dto.Content?.Length ?? dto.SizeInBytes,
                    DocumentType = dto.DocumentType,
                    CreateDate = dto.CreateDate,
                    CreatedByUserId = dto.CreatedByUserId,
                    IsEncrypted = dto.IsEncrypted,
                    Checksum = dto.Checksum,
                    CorrelationId = correlationId,
                    Status = dto.Status,
                    Content = dto.Content ?? Array.Empty<byte>()
                };

                _context.Documents.Add(entity);
                await _context.SaveChangesAsync();

                dto.Id = entity.Id;
                dto.CorrelationId = correlationId;
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create document: {Name}", dto.Name);
                throw;
            }
        }

        public async Task<DocumentDto?> GetDocumentByIdAsync(int documentId)
        {
            try
            {
                var doc = await _context.Documents.FindAsync(documentId);
                if (doc == null) return null;

                if (doc.IsEncrypted && doc.Content != null && doc.Content.Length > 0)
                {
                    try
                    {
                        doc.Content = _encryptionDocHelper.DecryptBytes(doc.Content);
                    }
                    catch (Exception decryptEx)
                    {
                        _logger.LogError(decryptEx, "Failed to decrypt content for DocumentId: {DocumentId}", documentId);
                        // Optional: decide whether to return null, throw, or continue with encrypted content
                    }
                }

                return MapToDto(doc);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document by ID: {DocumentId}", documentId);
                throw;
            }
        }

        // Replace the incorrect usage of ToListAsync with ToList
        public async Task<IEnumerable<DocumentDto>> GetAllAsync()
        {
            try
            {
                return await _context.Documents
                    .OrderByDescending(d => d.CreateDate)
                    .Select(d => MapToDto(d))
                    .ToListAsync(); // Ensure this is ToListAsync
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all documents.");
                throw;
            }
        }

        public async Task<bool> DeleteDocumentAsync(int documentId)
        {
            try
            {
                var doc = await _context.Documents.FindAsync(documentId);
                if (doc == null)
                {
                    _logger.LogWarning("Document not found for deletion: {DocumentId}", documentId);
                    return false;
                }

                _context.Documents.Remove(doc);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting document: {DocumentId}", documentId);
                throw;
            }
        }

        public async Task<IEnumerable<DocumentDto>> GetDocumentByPropertyIdAsync(int propertyId)
        {
            try
            {
                // Fix: Materialize the query before projecting to DTOs
                var docs = await _context.DocumentReferences
                    .Where(r => r.RelatedEntityType == "Property" && r.RelatedEntityId == propertyId)
                    .Select(r => r.Document)
                    .ToListAsync();

                return docs.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving documents by PropertyId: {PropertyId}", propertyId);
                throw;
            }
        }

        public async Task<IEnumerable<DocumentDto>> GetDocumentByTenantIdAsync(int tenantId)
        {
            try
            {
                var docs = await _context.DocumentReferences
                    .Where(r => r.RelatedEntityType == "Tenant" && r.RelatedEntityId == tenantId)
                    .Select(r => r.Document)
                    .ToListAsync();

                return docs.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving documents by TenantId: {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<IEnumerable<DocumentDto>> GetDocumentByOwnerIdAsync(int ownerId)
        {
            try
            {
                var docs = await _context.DocumentReferences
                    .Where(r => r.RelatedEntityType == "Owner" && r.RelatedEntityId == ownerId)
                    .Select(r => r.Document)
                    .ToListAsync();

                return docs.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving documents by OwnerId: {OwnerId}", ownerId);
                throw;
            }
        }

        public async Task<IEnumerable<DocumentDto>> GetDocumentByCreatedByAsync(int userId)
        {
            try
            {
                return await _context.Documents
                    .Where(d => d.CreatedByUserId == userId)
                    .Select(d => MapToDto(d))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving documents created by UserId: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<DocumentDto>> GetDocumentByAccessRoleAsync(string role)
        {
            try
            {
                var docs = await _context.DocumentReferences
                    .Where(r => r.AccessRole == role)
                    .Select(r => r.Document)
                    .ToListAsync();

                return docs.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving documents by AccessRole: {Role}", role);
                throw;
            }
        }

        public async Task<byte[]?> GetDocumentContentAsync(int documentId)
        {
            try
            {
                var doc = await _context.Documents
                    .Where(d => d.Id == documentId)
                    .Select(d => new { d.Content, d.IsEncrypted })
                    .FirstOrDefaultAsync();

                if (doc == null || doc.Content == null || doc.Content.Length == 0)
                    return null;

                return doc.IsEncrypted
                    ? _encryptionDocHelper.DecryptBytes(doc.Content)
                    : doc.Content;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving and decrypting content for DocumentId: {DocumentId}", documentId);
                throw;
            }
        }

        public async Task<bool> UploadDocumentContentAsync(int documentId, byte[] encryptedContent)
        {
            if (encryptedContent == null || encryptedContent.Length == 0)
            {
                _logger.LogWarning("Empty content passed for upload: {DocumentId}", documentId);
                return false;
            }

            for (int attempt = 1; attempt <= MaxRetryCount; attempt++)
            {
                try
                {
                    var doc = await _context.Documents.FindAsync(documentId);
                    if (doc == null)
                    {
                        _logger.LogWarning("Document not found for content upload: {DocumentId}", documentId);
                        return false;
                    }

                    // Store already encrypted content
                    doc.Content = encryptedContent;
                    doc.SizeInBytes = encryptedContent.Length;
                    doc.Checksum = DocumentHelper.GetChecksum(encryptedContent);
                    doc.IsEncrypted = true;

                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Document content uploaded successfully on attempt {Attempt}: {DocumentId}", attempt, documentId);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Attempt {Attempt} failed to upload content for DocumentId: {DocumentId}", attempt, documentId);

                    if (attempt == MaxRetryCount)
                    {
                        _logger.LogError(ex, "All upload attempts failed for DocumentId: {DocumentId}", documentId);
                        return false;
                    }

                    await Task.Delay(RetryDelay * attempt);
                }
            }

            return false;
        }

        private static DocumentDto MapToDto(Document d) => new DocumentDto
        {
            Id = d.Id,
            Name = d.Name,
            MimeType = d.MimeType,
            SizeInBytes = d.SizeInBytes,
            DocumentType = d.DocumentType,
            CreateDate = d.CreateDate,
            CreatedByUserId = d.CreatedByUserId,
            IsEncrypted = d.IsEncrypted,
            Checksum = d.Checksum,
            CorrelationId = d.CorrelationId,
            Status = d.Status,
            References = new List<DocumentReferenceDto>() // Populate via eager loading if needed
        };
    }
}