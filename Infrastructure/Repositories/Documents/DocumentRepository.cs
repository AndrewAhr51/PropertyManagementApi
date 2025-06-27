using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs.Documents;
using PropertyManagementAPI.Domain.Entities.Documents;
using PropertyManagementAPI.Infrastructure.Data;

public class DocumentRepository : IDocumentRepository
{
    private readonly MySqlDbContext _context;

    public DocumentRepository(MySqlDbContext context)
    {
        _context = context;
    }

    public async Task<DocumentDto> AddAsync(DocumentDto dto)
    {
        var propertyExists = await _context.Properties
       .AnyAsync(p => p.PropertyId == dto.PropertyId && p.IsActive);

        if (!propertyExists)
            throw new InvalidOperationException($"Property with ID {dto.PropertyId} does not exist or is not active.");

        var tenantExists = await _context.Tenants
       .AnyAsync(t => t.TenantId == dto.TenantId);

        if (!tenantExists)
            throw new InvalidOperationException($"Tenant with ID {dto.TenantId} does not exist.");
        var entity = new Document
        {
            PropertyId = dto.PropertyId,
            TenantId = dto.TenantId,
            FileName = dto.FileName,
            FileUrl = dto.FileUrl,
            Category = dto.Category,
        };

        _context.Documents.Add(entity);
        await _context.SaveChangesAsync();

        dto.DocumentId = entity.DocumentId;
        dto.CreatedBy = entity.CreatedBy;
        return dto;
    }

    public async Task<IEnumerable<DocumentDto>> GetAllAsync()
    {
        return await _context.Documents
            .Select(d => new DocumentDto
            {
                DocumentId = d.DocumentId,
                PropertyId = d.PropertyId,
                TenantId = d.TenantId,
                FileName = d.FileName,
                FileUrl = d.FileUrl,
                Category = d.Category,
                CreatedDate = d.CreatedDate,
                CreatedBy = d.CreatedBy
            })
            .ToListAsync();
    }

    public async Task<DocumentDto?> GetByIdAsync(int documentId)
    {
        var d = await _context.Documents.FindAsync(documentId);
        return d == null ? null : new DocumentDto
        {
            DocumentId = d.DocumentId,
            PropertyId = d.PropertyId,
            TenantId = d.TenantId,
            FileName = d.FileName,
            FileUrl = d.FileUrl,
            Category = d.Category,
            CreatedDate = d.CreatedDate,
            CreatedBy = d.CreatedBy
        };
    }

    public async Task<bool> DeleteAsync(int documentId)
    {
        var entity = await _context.Documents.FindAsync(documentId);
        if (entity == null) return false;

        _context.Documents.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<IEnumerable<DocumentDto>> GetByPropertyIdAsync(int propertyId)
{
    return await _context.Documents
        .Where(d => d.PropertyId == propertyId)
        .Select(d => new DocumentDto
        {
            DocumentId = d.DocumentId,
            PropertyId = d.PropertyId,
            TenantId = d.TenantId,
            FileName = d.FileName,
            FileUrl = d.FileUrl,
            Category = d.Category,
            CreatedDate = d.CreatedDate,
            CreatedBy = d.CreatedBy
        })
        .ToListAsync();
}

public async Task<IEnumerable<DocumentDto>> GetByTenantIdAsync(int tenantId)
{
    return await _context.Documents
        .Where(d => d.TenantId == tenantId)
        .Select(d => new DocumentDto
        {
            DocumentId = d.DocumentId,
            PropertyId = d.PropertyId,
            TenantId = d.TenantId,
            FileName = d.FileName,
            FileUrl = d.FileUrl,
            Category = d.Category,
            CreatedDate = d.CreatedDate,
            CreatedBy = d.CreatedBy
        })
        .ToListAsync();
}
}