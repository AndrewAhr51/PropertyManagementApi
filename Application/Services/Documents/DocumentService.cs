using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Common.Utilities;
using PropertyManagementAPI.Domain.DTOs.Documents;
using PropertyManagementAPI.Domain.Entities.User;
using PropertyManagementAPI.Infrastructure.Repositories.Documents;
using System.ComponentModel.Design;

namespace PropertyManagementAPI.Application.Services.Documents
{

    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentReferenceService _referenceService;
        private readonly ILogger<DocumentService> _logger;
        private readonly EncryptionDocHelper _encryptionDocHelper;

        public DocumentService(IDocumentRepository documentRepository, ILogger<DocumentService> logger, IDocumentReferenceService referenceService, EncryptionDocHelper encryptionDocHelper)
        {
            _documentRepository = documentRepository;
            _referenceService = referenceService;
            _logger = logger;
            _encryptionDocHelper = encryptionDocHelper;
        }

        public async Task<DocumentDto> CreateDocumentAsync(DocumentDto dto)
        {
            try
            {
                _logger.LogInformation("Creating document: {Name}", dto.Name);

                var created = await _documentRepository.CreateDocumentAsync(dto); // Handles metadata only
                if (created?.Id <= 0)
                    throw new InvalidOperationException("Document creation failed.");

                if (dto.Content?.Length > 0)
                {
                    var updated = await _documentRepository.UploadDocumentContentAsync(created.Id, dto.Content);

                    if (updated == null)
                    {
                        _logger.LogWarning("Content upload failed for DocumentId: {DocumentId}", created.Id);
                        // Optional: return metadata-only if binary failed
                        return created;
                    }

                    created = updated; // Replace with updated content, size, checksum, etc.
                }

                _logger.LogInformation("Document created successfully: {DocumentId}", created.Id);
                return created;
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
                _logger.LogInformation("Fetching document by ID: {DocumentId}", documentId);
                return await _documentRepository.GetDocumentByIdAsync(documentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching document ID: {DocumentId}", documentId);
                throw;
            }
        }

        public async Task<IEnumerable<DocumentDto>> GetAllDocumentAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all documents");
                return await _documentRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving documents");
                throw;
            }
        }

        public async Task<bool> DeleteDocumentAsync(int documentId)
        {
            try
            {
                _logger.LogWarning("Deleting document ID: {DocumentId}", documentId);
                return await _documentRepository.DeleteDocumentAsync(documentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete document ID: {DocumentId}", documentId);
                throw;
            }
        }

        public async Task<IEnumerable<DocumentDto>> GetDocumentByPropertyIdAsync(int propertyId)
        {
            try
            {
                _logger.LogInformation("Retrieving documents for property ID: {PropertyId}", propertyId);
                return await _documentRepository.GetDocumentByPropertyIdAsync(propertyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve documents for property ID: {PropertyId}", propertyId);
                throw;
            }
        }

        public async Task<IEnumerable<DocumentDto>> GetDocumentByTenantIdAsync(int tenantId)
        {
            try
            {
                _logger.LogInformation("Retrieving documents for tenant ID: {TenantId}", tenantId);
                return await _documentRepository.GetDocumentByTenantIdAsync(tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving documents for tenant ID: {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<IEnumerable<DocumentDto>> GetDocumentByOwnerIdAsync(int ownerId)
        {
            try
            {
                _logger.LogInformation("Retrieving documents for owner ID: {OwnerId}", ownerId);
                return await _documentRepository.GetDocumentByOwnerIdAsync(ownerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving documents for owner ID: {OwnerId}", ownerId);
                throw;
            }
        }

        public async Task<IEnumerable<DocumentDto>> GetDocumentByCreatedByAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Retrieving documents created by user ID: {UserId}", userId);
                return await _documentRepository.GetDocumentByCreatedByAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving documents for creator ID: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<DocumentDto>> GetDocumentByAccessRoleAsync(string role)
        {
            try
            {
                _logger.LogInformation("Retrieving documents with access role: {Role}", role);
                return await _documentRepository.GetDocumentByAccessRoleAsync(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving documents with role: {Role}", role);
                throw;
            }
        }

        public async Task<byte[]?> GetDocumentContentAsync(int documentId)
        {
            try
            {
                _logger.LogInformation("Retrieving document content for ID: {DocumentId}", documentId);
                return await _documentRepository.GetDocumentContentAsync(documentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document content for ID: {DocumentId}", documentId);
                throw;
            }
        }

        public async Task<DocumentDto> UploadCompleteDocumentAsync(DocumentUploadDto uploadDto)
        {
            try
            {
                _logger.LogInformation("Initiating full document upload for: {FileName}", uploadDto.FileName);

                var encryptedContent = _encryptionDocHelper.EncryptBytes(uploadDto.Content);

                // 1️⃣ Generate correlation ID internally
                var correlationId = Guid.NewGuid().ToString();

                var metadata = new DocumentDto
                {
                    Name = uploadDto.FileName,
                    Content = encryptedContent,
                    MimeType = MimeTypeResolver.GetMimeType(uploadDto.FileName, _logger),
                    SizeInBytes = encryptedContent.Length,
                    DocumentType = uploadDto.DocumentType ?? "General",
                    CreateDate = DateTime.UtcNow,
                    CreatedBy = uploadDto.UploadedByUserId,
                    IsEncrypted = true,
                    Status = uploadDto.Status,
                    Checksum = DocumentHelper.GetChecksum(encryptedContent),
                    CorrelationId = correlationId
                };

                _logger.LogInformation("Document uploaded: {FileName} | MIME: {MimeType} | Checksum: {Checksum}", uploadDto.FileName, metadata.MimeType, metadata.Checksum);

                // Step 2️ Validate metadata
                if (string.IsNullOrWhiteSpace(metadata.Name) || metadata.SizeInBytes <= 0)
                {
                    _logger.LogWarning("Invalid document metadata for: {FileName}", uploadDto.FileName);
                    throw new ArgumentException("Invalid document metadata provided.");
                }
                if (string.IsNullOrWhiteSpace(metadata.MimeType))
                {
                    metadata.MimeType = MimeTypeResolver.GetMimeType(uploadDto.FileName, _logger);
                    _logger.LogInformation("Resolved MIME type for {FileName}: {MimeType}", uploadDto.FileName, metadata.MimeType);
                }
                if (metadata.SizeInBytes <= 0)
                {
                    _logger.LogWarning("Document size is zero or negative for: {FileName}", uploadDto.FileName);
                    throw new ArgumentException("Document size must be greater than zero.");
                }
                if (string.IsNullOrWhiteSpace(metadata.DocumentType))
                {
                    metadata.DocumentType = "General"; // Default type
                    _logger.LogInformation("Default document type set for {FileName}: {DocumentType}", uploadDto.FileName, metadata.DocumentType);
                }
                if (uploadDto.UploadedByUserId <= 0)
                {
                    _logger.LogWarning("Invalid user ID for upload: {FileName}", uploadDto.FileName);
                    throw new ArgumentException("UploadedByUserId must be a valid user ID.");
                }
                if (string.IsNullOrWhiteSpace(uploadDto.Status))
                {
                    metadata.Status = "Active"; // Default status
                    _logger.LogInformation("Default status set for {FileName}: {Status}", uploadDto.FileName, metadata.Status);
                }

                // Step 3️ Create document (metadata only)
                var created = await _documentRepository.CreateDocumentAsync(metadata);
                if (created.Id <= 0)
                    throw new InvalidOperationException("Document creation failed.");

                // Step 4️ Upload content
                created = await _documentRepository.UploadDocumentContentAsync(created.Id, encryptedContent);
                if (created.Id <= 0)
                    throw new InvalidOperationException("Document creation failed.");

                // Step 5️ Optionally auto-link to default entity (e.g. tenant, owner) if desired
                await _referenceService.AddReferenceAsync(new DocumentReferenceDto
                {
                    DocumentId = created.Id,
                    RelatedEntityType = uploadDto.DocumentType,
                    RelatedEntityId = uploadDto.UploadedByUserId,
                    LinkedDate = DateTime.UtcNow,
                    AccessRole = "Manager",
                    Description = uploadDto.Description
                });

                _logger.LogInformation("Document fully uploaded: {DocumentId}", created.Id);
                return created;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to complete document upload for: {FileName}", uploadDto.FileName);
                throw;
            }
        }
    }
}