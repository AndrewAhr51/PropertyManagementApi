using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Data;

public class LeaseRepository : ILeaseRepository
{
    private readonly AppDbContext _context;

    public LeaseRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LeaseDto> AddAsync(LeaseDto dto)
    {
        var propertyExists = await _context.Property
        .AnyAsync(p => p.PropertyId == dto.PropertyId && p.IsActive);

        if (!propertyExists)
            throw new InvalidOperationException($"Property with ID {dto.PropertyId} does not exist or is not active.");

        var tenantExists = await _context.Tenants
       .AnyAsync(t => t.TenantId == dto.TenantId);

        if (!tenantExists)
            throw new InvalidOperationException($"Tenant with ID {dto.TenantId} does not exist.");

        var entity = new Lease
        {
            TenantId = dto.TenantId,
            PropertyId = dto.PropertyId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            MonthlyRent = dto.MonthlyRent,
            DepositPaid = dto.DepositPaid,
            IsActive = true,
            SignedDate = dto.SignedDate == default ? DateTime.UtcNow : dto.SignedDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.Leases.Add(entity);
        await _context.SaveChangesAsync();

        dto.LeaseId = entity.LeaseId;
        dto.CreatedAt = entity.CreatedAt;
        return dto;
    }

    public async Task<IEnumerable<LeaseDto>> GetAllAsync()
    {
        return await _context.Leases
            .Where(l => l.IsActive)
            .Select(l => new LeaseDto
            {
                LeaseId = l.LeaseId,
                TenantId = l.TenantId,
                PropertyId = l.PropertyId,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                MonthlyRent = l.MonthlyRent,
                DepositPaid = l.DepositPaid,
                IsActive = l.IsActive,
                SignedDate = l.SignedDate,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<LeaseDto?> GetByIdAsync(int leaseId)
    {
        var l = await _context.Leases
            .Where(l => l.LeaseId == leaseId && l.IsActive)
            .FirstOrDefaultAsync();

        return l == null ? null : new LeaseDto
        {
            LeaseId = l.LeaseId,
            TenantId = l.TenantId,
            PropertyId = l.PropertyId,
            StartDate = l.StartDate,
            EndDate = l.EndDate,
            MonthlyRent = l.MonthlyRent,
            DepositPaid = l.DepositPaid,
            IsActive = l.IsActive,
            SignedDate = l.SignedDate,
            CreatedAt = l.CreatedAt
        };
    }

    public async Task<bool> UpdateAsync(int leaseId, LeaseDto dto)
    {
        var entity = await _context.Leases.FindAsync(leaseId);
        if (entity == null || !entity.IsActive) return false;

        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.MonthlyRent = dto.MonthlyRent;
        entity.DepositPaid = dto.DepositPaid;
        entity.SignedDate = dto.SignedDate;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int leaseId)
    {
        var entity = await _context.Leases.FindAsync(leaseId);
        if (entity == null || !entity.IsActive) return false;

        entity.IsActive = !entity.IsActive;
        await _context.SaveChangesAsync();
        return true;
    }
}