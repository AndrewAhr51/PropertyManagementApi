using PropertyManagementAPI.Domain.DTOs;

namespace PropertyManagementAPI.Application.Services.Documents
{
    public interface IDocumentStorageService
    {
        Task<DocumentStorageDto> GetDocumentByIdAsync(int documentStorageId);
        Task<IEnumerable<DocumentStorageDto>> GetAllDocumentsAsync();
        Task<bool> UploadDocumentAsync(DocumentStorageDto documentStorageDto);
        Task<bool> DeleteDocumentAsync(int documentStorageId);
    }
}
