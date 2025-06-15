using PropertyManagementAPI.Domain.DTOs;

public class DocumentService : IDocumentService
{
    private readonly IDocumentRepository _repository;

    public DocumentService(IDocumentRepository repository)
    {
        _repository = repository;
    }

    public async Task<DocumentDto> CreateAsync(DocumentDto dto)
    {
        return await _repository.AddAsync(dto);
    }

    public async Task<IEnumerable<DocumentDto>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<DocumentDto?> GetByIdAsync(int documentId)
    {
        return await _repository.GetByIdAsync(documentId);
    }

    public async Task<IEnumerable<DocumentDto>> GetByPropertyIdAsync(int propertyId)
    {
        return await _repository.GetByPropertyIdAsync(propertyId);
    }

    public async Task<IEnumerable<DocumentDto>> GetByTenantIdAsync(int tenantId)
    {
        return await _repository.GetByTenantIdAsync(tenantId);
    }

    public async Task<bool> DeleteAsync(int documentId)
    {
        return await _repository.DeleteAsync(documentId);
    }
}