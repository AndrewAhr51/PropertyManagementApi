using PropertyManagementAPI.Domain.DTOs.Documents;

public interface IDocumentRepository
{
    Task<DocumentDto> AddAsync(DocumentDto dto);
    Task<IEnumerable<DocumentDto>> GetAllAsync();
    Task<DocumentDto?> GetByIdAsync(int documentId);
    Task<bool> DeleteAsync(int documentId);
    Task<IEnumerable<DocumentDto>> GetByPropertyIdAsync(int propertyId);
    Task<IEnumerable<DocumentDto>> GetByTenantIdAsync(int tenantId);
}