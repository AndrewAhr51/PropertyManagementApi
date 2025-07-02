using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs.Property.Leases;
using PropertyManagementAPI.Domain.Entities.Property;
using PropertyManagementAPI.Domain.Entities.User;
using PropertyManagementAPI.Infrastructure.Data;

public class LeaseRepository : ILeaseRepository
{
    private readonly MySqlDbContext _context;

    public LeaseRepository(MySqlDbContext context)
    {
        _context = context;
    }

    public async Task<LeaseDto> CreateLeaseAsync(LeaseDto dto)
    {
        var propertyExists = await _context.Properties
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
            DepositAmount = dto.DepositAmount,
            DepositPaid = dto.DepositPaid,
            IsActive = true,
            SignedDate = dto.SignedDate == default ? DateTime.UtcNow : dto.SignedDate,
        };

        _context.Leases.Add(entity);
        await _context.SaveChangesAsync();

        dto.LeaseId = entity.LeaseId;
        dto.CreatedBy = entity.CreatedBy;
        return dto;
    }

    public async Task<IEnumerable<LeaseDto>> GetAllLeasesAsync()
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
                CreatedBy = l.CreatedBy
            })
            .ToListAsync();
    }

    public async Task<LeaseDto?> GetLeaseByIdAsync(int leaseId)
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
            CreatedBy = l.CreatedBy
        };
    }
    
    public async Task<bool> UpdateLeaseByIdAsync(LeaseUpdateDto dto)
    {
        var entity = await _context.Leases.FindAsync(dto.LeaseId);
        if (entity == null || !entity.IsActive) return false;

        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.DepositAmount = dto.DepositAmount;
        entity.MonthlyRent = dto.MonthlyRent;
        entity.DepositPaid = dto.DepositPaid;
        entity.SignedDate = dto.SignedDate;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteLeaseByIdAsync(int leaseId)
    {
        var entity = await _context.Leases.FindAsync(leaseId);
        if (entity == null || !entity.IsActive) return false;

        entity.IsActive = !entity.IsActive;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<LeaseDto?> GetLeaseByTenantIdAsync(int tenantId)
    {
        var lease = await _context.Leases
          .Where(l => l.TenantId == tenantId && l.IsActive)
          .Include(lease => lease.Tenants)
          .Include(lease => lease.Property)
          .FirstOrDefaultAsync();

        return lease == null ? null : new LeaseDto
        {
            LeaseId = lease.LeaseId,
            PropertyId = lease.PropertyId,
            PropertyName = lease.Property.PropertyName,
            TenantId = lease.TenantId,
            TenantFirstName = lease.Tenants.FirstName,
            TenantLastName = lease.Tenants.LastName,
            TenantPhoneNumber = lease.Tenants.PhoneNumber,
            StartDate = lease.StartDate,
            EndDate = lease.EndDate,
            MonthlyRent = lease.MonthlyRent,
            DepositPaid = lease.DepositPaid,
            IsActive = lease.IsActive,
            SignedDate = lease.SignedDate,
            CreatedBy = lease.CreatedBy
        };
    }

    public async Task<IEnumerable<LeaseDto>> GetAllLeasesByOwnerIdAsync(int ownerId)
    {
        var leases = await _context.Leases
        .Where(lease => lease.Property.PropertyOwners.Any(po => po.OwnerId == ownerId))
        .Include(lease => lease.Tenants)
        .Include(lease => lease.Property)
        .Select(lease => new LeaseDto
        {
          LeaseId = lease.LeaseId,
          TenantId = lease.TenantId,
          TenantFirstName = lease.Tenants.FirstName,
          TenantLastName = lease.Tenants.LastName,
          TenantPhoneNumber = lease.Tenants.PhoneNumber,
          PropertyId = lease.PropertyId,
          PropertyName = lease.Property.PropertyName,
          StartDate = lease.StartDate,
          EndDate = lease.EndDate,
          MonthlyRent = lease.MonthlyRent,
          DepositAmount = lease.DepositAmount,
          DepositPaid = lease.DepositPaid,
          IsActive = lease.IsActive,
          SignedDate = lease.SignedDate,
          CreatedBy = lease.CreatedBy
        })
        .ToListAsync();

        return (IEnumerable<LeaseDto>)leases;
    }
}