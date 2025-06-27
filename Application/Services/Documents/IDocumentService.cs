using PropertyManagementAPI.Domain.DTOs.Documents;

public interface IDocumentService
{
    Task<DocumentDto> CreateAsync(DocumentDto dto);
    Task<IEnumerable<DocumentDto>> GetAllAsync();
    Task<DocumentDto?> GetByIdAsync(int documentId);
    Task<IEnumerable<DocumentDto>> GetByPropertyIdAsync(int propertyId);
    Task<IEnumerable<DocumentDto>> GetByTenantIdAsync(int tenantId);
    Task<bool> DeleteAsync(int documentId);
}