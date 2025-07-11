using PropertyManagementAPI.Domain.DTOs.Documents;

namespace PropertyManagementAPI.Infrastructure.Repositories.Documents
{
    public interface IDocumentRepository
    {
        // Core Operations
        Task<DocumentDto> CreateDocumentAsync(DocumentDto dto);
        Task<DocumentDto?> GetDocumentByIdAsync(int documentId);
        Task<IEnumerable<DocumentDto>> GetAllAsync();
        Task<bool> DeleteDocumentAsync(int documentId);

        // Entity-Specific Filters
        Task<IEnumerable<DocumentDto>> GetDocumentByPropertyIdAsync(int propertyId);
        Task<IEnumerable<DocumentDto>> GetDocumentByTenantIdAsync(int tenantId);
        Task<IEnumerable<DocumentDto>> GetDocumentByOwnerIdAsync(int ownerId);

        // Audit & Access Filters
        Task<IEnumerable<DocumentDto>> GetDocumentByCreatedByAsync(int userId);
        Task<IEnumerable<DocumentDto>> GetDocumentByAccessRoleAsync(string role);

        // Binary Access
        Task<byte[]?> GetDocumentContentAsync(int documentId);
        Task<bool> UploadDocumentContentAsync(int documentId, byte[] content);
    }
}