using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Repositories.Documents;

namespace PropertyManagementAPI.Application.Services.Documents
{
    public class DocumentStorageService : IDocumentStorageService
    {
        private readonly IDocumentStorageRepository _documentStorageRepository;

        public DocumentStorageService(IDocumentStorageRepository documentStorageRepository)
        {
            _documentStorageRepository = documentStorageRepository;
        }

        public async Task<DocumentStorageDto> GetDocumentByIdAsync(int documentStorageId)
        {
            var documentStorage = await _documentStorageRepository.GetDocumentByIdAsync(documentStorageId);
            return documentStorage != null ? MapToDto(documentStorage) : null;
        }

        public async Task<IEnumerable<DocumentStorageDto>> GetAllDocumentsAsync()
        {
            var documents = await _documentStorageRepository.GetAllDocumentsAsync();
            return documents.Select(MapToDto).ToList();
        }

        public async Task<bool> UploadDocumentAsync(DocumentStorageDto documentStorageDto)
        {
            var documentStorage = MapToEntity(documentStorageDto);
            return await _documentStorageRepository.AddDocumentAsync(documentStorage);
        }

        public async Task<bool> DeleteDocumentAsync(int documentStorageId) =>
            await _documentStorageRepository.DeleteDocumentAsync(documentStorageId);

        private DocumentStorageDto MapToDto(DocumentStorage documentStorage)
        {
            return new DocumentStorageDto
            {
                DocumentStorageId = documentStorage.DocumentStorageId,
                DocumentId = documentStorage.DocumentId,
                FileName = documentStorage.FileName,
                DocumentTypeId = documentStorage.DocumentTypeId,
                FileData = documentStorage.FileData,
                CreatedDate = documentStorage.CreatedDate
            };
        }

        private DocumentStorage MapToEntity(DocumentStorageDto dto)
        {
            return new DocumentStorage
            {
                DocumentStorageId = dto.DocumentStorageId,
                DocumentId = dto.DocumentId,
                FileName = dto.FileName,
                DocumentTypeId = dto.DocumentTypeId,
                FileData = dto.FileData,
                CreatedDate = dto.CreatedDate
            };
        }
    }
}
