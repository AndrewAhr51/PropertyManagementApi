using PropertyManagementAPI.Domain.Entities.Documents;

namespace PropertyManagementAPI.Infrastructure.Repositories.Documents
{
    public interface IDocumentStorageRepository
    {
        Task<DocumentStorage> GetDocumentByIdAsync(int documentStorageId);
        Task<IEnumerable<DocumentStorage>> GetAllDocumentsAsync();
        Task<bool> AddDocumentAsync(DocumentStorage documentStorage);
        Task<bool> DeleteDocumentAsync(int documentStorageId);
    }
}
