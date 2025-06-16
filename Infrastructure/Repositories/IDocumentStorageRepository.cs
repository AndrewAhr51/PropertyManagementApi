using PropertyManagementAPI.Domain.Entities;

namespace PropertyManagementAPI.Infrastructure.Repositories
{
    public interface IDocumentStorageRepository
    {
        Task<DocumentStorage> GetDocumentByIdAsync(int documentStorageId);
        Task<IEnumerable<DocumentStorage>> GetAllDocumentsAsync();
        Task<bool> AddDocumentAsync(DocumentStorage documentStorage);
        Task<bool> DeleteDocumentAsync(int documentStorageId);
    }
}
