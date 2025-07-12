using PropertyManagementAPI.Domain.DTOs.Documents;

namespace PropertyManagementAPI.Infrastructure.Repositories.Documents
{
    public interface IDocumentReferenceRepository
    {
        Task<DocumentReferenceDto?> GetDocumentReferenceByIdAsync(int referenceId);
        Task<IEnumerable<DocumentReferenceDto>> GetDocumentReferenceByDocumentIdAsync(int documentId);
        Task<IEnumerable<DocumentReferenceDto>> GetDocumentReferenceByEntityAsync(string relatedEntityType, int relatedEntityId);
        Task<IEnumerable<DocumentReferenceDto>> GetDocumentReferenceByCreatorAsync(int userId);

        Task<bool> AddDocumentReferenceAsync(DocumentReferenceDto referenceDto);
        Task<bool> RemoveDocumentReferenceAsync(int referenceId);
    }
}

