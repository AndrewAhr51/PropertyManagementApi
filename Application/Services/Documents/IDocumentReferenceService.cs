using PropertyManagementAPI.Domain.DTOs.Documents;

namespace PropertyManagementAPI.Application.Services.Documents
{
    public interface IDocumentReferenceService
    {
        Task<IEnumerable<DocumentReferenceDto>> GetReferencesByDocumentIdAsync(int documentId);
        Task<IEnumerable<DocumentReferenceDto>> GetReferencesByEntityAsync(string relatedEntityType, int relatedEntityId);
        Task<DocumentReferenceDto?> GetByIdAsync(int referenceId);
        Task<DocumentReferenceDto> AddReferenceAsync(DocumentReferenceDto referenceDto);
        Task<bool> RemoveReferenceAsync(int referenceId);
        Task<IEnumerable<DocumentReferenceDto>> GetReferencesCreatedByAsync(int userId);
    }
}