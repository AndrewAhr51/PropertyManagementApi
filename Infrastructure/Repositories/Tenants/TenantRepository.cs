using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs.Users;
using PropertyManagementAPI.Domain.Entities.User;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Repositories.Users;

public class TenantRepository : ITenantRepository
{
    private readonly MySqlDbContext _context;
    private readonly ILogger<TenantRepository> _logger;

    public TenantRepository(MySqlDbContext context, ILogger<TenantRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TenantDto> AddTenantAsync(TenantDto dto)
    {
        var entity = new Tenant
        {
            TenantId = dto.UserId,
            PropertyId = dto.PropertyId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            MoveInDate = dto.MoveInDate,
            Balance = 0.0m, // Default balance
        };

        _context.Tenants.Add(entity);
        await _context.SaveChangesAsync();

        dto.TenantId = entity.TenantId;
        dto.CreatedBy = entity.CreatedBy;
        return dto;
    }

    public async Task<IEnumerable<TenantDto>> GetAllTenantsAsync()
    {
        return await _context.Tenants
            .Select(t => new TenantDto
            {
                TenantId = t.TenantId,
                PropertyId = t.PropertyId,
                FirstName = t.FirstName,
                LastName = t.LastName,
                PhoneNumber = t.PhoneNumber,
                MoveInDate = t.MoveInDate,
                CreatedBy = t.CreatedBy
            })
            .ToListAsync();
    }

    public async Task<TenantDto?> GetTenantByIdAsync(int tenantId)
    {
        var t = await _context.Tenants.FindAsync(tenantId);
        return t == null ? null : new TenantDto
        {
            TenantId = t.TenantId,
            PropertyId = t.PropertyId,
            FirstName = t.FirstName,
            LastName = t.LastName,
            PhoneNumber = t.PhoneNumber,
            MoveInDate = t.MoveInDate,
            CreatedBy = t.CreatedBy
        };
    }

    public async Task<bool> UpdateTenantAsync(int tenantId, TenantDto dto)
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

    public async Task<bool> SetActivateTenant(int tenantId)
    {
        try
        {
            var tenant = await _context.Tenants.FindAsync(tenantId);
            if (tenant == null)
            {
                _logger.LogWarning("User not found for ID: {Id}", tenantId);
                return false;
            }

            tenant.IsActive = !tenant.IsActive;
            var save = await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(tenantId);
            if (user == null)
            {
                _logger.LogWarning("User not found for ID: {Id}", tenantId);
                return false;
            }

            user.IsActive = !user.IsActive;
            save = await _context.SaveChangesAsync();

            return save > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inactivating the user with ID: {Id}", tenantId);
            throw new Exception("An error occurred while deleting the user.", ex);
        }
    }
}