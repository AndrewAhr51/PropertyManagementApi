using Microsoft.Extensions.Logging;
using PropertyManagementAPI.Domain.DTOs.Documents;
using PropertyManagementAPI.Infrastructure.Repositories.Documents;

namespace PropertyManagementAPI.Application.Services.Documents
{
    public class DocumentReferenceService : IDocumentReferenceService
    {
        private readonly IDocumentReferenceRepository _repository;
        private readonly ILogger<DocumentReferenceService> _logger;

        public DocumentReferenceService(
            IDocumentReferenceRepository repository,
            ILogger<DocumentReferenceService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<DocumentReferenceDto?> AddReferenceAsync(DocumentReferenceDto dto)
        {
            try
            {
                _logger.LogInformation("Adding reference to DocumentId: {DocumentId}", dto.DocumentId);

                var success = await _repository.AddDocumentReferenceAsync(dto);

                if (success)
                {
                    _logger.LogInformation("Reference added successfully. ReferenceId: {ReferenceId}", dto.Id);
                    return dto;
                }
                else
                {
                    _logger.LogWarning("Failed to add reference for DocumentId: {DocumentId}", dto.DocumentId);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding document reference for DocumentId: {DocumentId}", dto.DocumentId);
                return null;
            }
        }

        public async Task<IEnumerable<DocumentReferenceDto>> GetReferencesByDocumentIdAsync(int documentId)
        {
            try
            {
                _logger.LogInformation("Fetching references for DocumentId: {DocumentId}", documentId);
                return await _repository.GetDocumentReferenceByDocumentIdAsync(documentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching references for DocumentId: {DocumentId}", documentId);
                throw;
            }
        }

        public async Task<IEnumerable<DocumentReferenceDto>> GetReferencesByEntityAsync(string relatedEntityType, int relatedEntityId)
        {
            try
            {
                _logger.LogInformation("Fetching references for Entity {Type}:{Id}", relatedEntityType, relatedEntityId);
                return await _repository.GetDocumentReferenceByEntityAsync(relatedEntityType, relatedEntityId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching references for entity {Type}:{Id}", relatedEntityType, relatedEntityId);
                throw;
            }
        }

        public async Task<IEnumerable<DocumentReferenceDto>> GetReferencesCreatedByAsync(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching references created by UserId: {UserId}", userId);
                return await _repository.GetDocumentReferenceByCreatorAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching references created by UserId: {UserId}", userId);
                throw;
            }
        }

        public async Task<DocumentReferenceDto?> GetByIdAsync(int referenceId)
        {
            try
            {
                _logger.LogInformation("Fetching DocumentReference by ID: {ReferenceId}", referenceId);
                return await _repository.GetDocumentReferenceByIdAsync(referenceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching DocumentReference by ID: {ReferenceId}", referenceId);
                throw;
            }
        }

        public async Task<bool> RemoveReferenceAsync(int referenceId)
        {
            try
            {
                _logger.LogWarning("Removing DocumentReference ID: {ReferenceId}", referenceId);
                return await _repository.RemoveDocumentReferenceAsync(referenceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing DocumentReference ID: {ReferenceId}", referenceId);
                throw;
            }
        }
    }
}