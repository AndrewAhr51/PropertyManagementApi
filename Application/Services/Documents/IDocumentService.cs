using PropertyManagementAPI.Domain.DTOs.Documents;

namespace PropertyManagementAPI.Application.Services.Documents
{
    public interface IDocumentService
    {
        // Core Document Lifecycle
        Task<DocumentDto> CreateDocumentAsync(DocumentDto dto);
        Task<DocumentDto?> GetDocumentByIdAsync(int documentId);
        Task<IEnumerable<DocumentDto>> GetAllDocumentAsync();
        Task<bool> DeleteDocumentAsync(int documentId);

        // Entity-Specific Document Queries
        Task<IEnumerable<DocumentDto>> GetDocumentByPropertyIdAsync(int propertyId);
        Task<IEnumerable<DocumentDto>> GetDocumentByTenantIdAsync(int tenantId);
        Task<IEnumerable<DocumentDto>> GetDocumentByOwnerIdAsync(int ownerId);

        // Audit & Filtering
        Task<IEnumerable<DocumentDto>> GetDocumentByCreatedByAsync(int userId);
        Task<IEnumerable<DocumentDto>> GetDocumentByAccessRoleAsync(string role);

        // Binary Content Access (optional, for download APIs)
        Task<byte[]?> GetDocumentContentAsync(int documentId);
        Task<DocumentDto> UploadCompleteDocumentAsync(DocumentUploadDto uploadDto);
    }
}