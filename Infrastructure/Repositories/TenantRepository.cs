using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Data;

public class TenantRepository : ITenantRepository
{
    private readonly AppDbContext _context;

    public TenantRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<TenantDto> AddAsync(TenantDto dto)
    {
        var entity = new Tenant
        {
            PropertyId = dto.PropertyId,
            UserId = dto.UserId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            MoveInDate = dto.MoveInDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.Tenants.Add(entity);
        await _context.SaveChangesAsync();

        dto.TenantId = entity.TenantId;
        dto.CreatedAt = entity.CreatedAt;
        return dto;
    }

    public async Task<IEnumerable<TenantDto>> GetAllAsync()
    {
        return await _context.Tenants
            .Select(t => new TenantDto
            {
                TenantId = t.TenantId,
                PropertyId = t.PropertyId,
                UserId = t.UserId,
                FirstName = t.FirstName,
                LastName = t.LastName,
                PhoneNumber = t.PhoneNumber,
                MoveInDate = t.MoveInDate,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<TenantDto?> GetByIdAsync(int tenantId)
    {
        var t = await _context.Tenants.FindAsync(tenantId);
        return t == null ? null : new TenantDto
        {
            TenantId = t.TenantId,
            PropertyId = t.PropertyId,
            UserId = t.UserId,
            FirstName = t.FirstName,
            LastName = t.LastName,
            PhoneNumber = t.PhoneNumber,
            MoveInDate = t.MoveInDate,
            CreatedAt = t.CreatedAt
        };
    }

    public async Task<bool> UpdateAsync(int tenantId, TenantDto dto)
    {
        var entity = await _context.Tenants.FindAsync(tenantId);
        if (entity == null) return false;

        entity.FirstName = dto.FirstName;
        entity.LastName = dto.LastName;
        entity.PhoneNumber = dto.PhoneNumber;
        entity.MoveInDate = dto.MoveInDate;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int tenantId)
    {
        var entity = await _context.Tenants.FindAsync(tenantId);
        if (entity == null) return false;

        _context.Tenants.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}