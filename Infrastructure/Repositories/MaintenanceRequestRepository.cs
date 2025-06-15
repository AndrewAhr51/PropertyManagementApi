using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Infrastructure.Data;

public class MaintenanceRequestRepository : IMaintenanceRequestRepository
{
    private readonly AppDbContext _context;

    public MaintenanceRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<MaintenanceRequestDto> AddAsync(MaintenanceRequestDto dto)
    {
        var userExists = await _context.Users.AnyAsync(u => u.UserId == dto.UserId);
        if (!userExists)
            throw new InvalidOperationException($"User with ID {dto.UserId} does not exist.");

        var propertyExists = await _context.Property.AnyAsync(p => p.PropertyId == dto.PropertyId);
        if (!propertyExists)
            throw new InvalidOperationException($"Property with ID { dto.PropertyId} does not exist.");

        var entity = new MaintenanceRequests
        {
            UserId = dto.UserId,
            PropertyId = dto.PropertyId,
            RequestDate = dto.RequestDate == default ? DateTime.UtcNow : dto.RequestDate,
            Category = dto.Category,
            Description = dto.Description,
            PriorityLevel = dto.PriorityLevel ?? "Normal",
            Status = "Open",
            AssignedTo = string.Empty,
            ResolutionNotes = string.Empty,
            ResolvedDate = null,
            LastUpdated = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _context.MaintenanceRequests.Add(entity);
        await _context.SaveChangesAsync();

        dto.RequestId = entity.RequestId;
        dto.CreatedAt = entity.CreatedAt;
        return dto;
    }

    public async Task<IEnumerable<MaintenanceRequestDto>> GetAllAsync()
    {
        return await _context.MaintenanceRequests
            .Select(r => new MaintenanceRequestDto
            {
                RequestId = r.RequestId,
                UserId = r.UserId,
                PropertyId = r.PropertyId,
                RequestDate = r.RequestDate,
                Category = r.Category,
                Description = r.Description,
                PriorityLevel = r.PriorityLevel,
                Status = r.Status,
                AssignedTo = r.AssignedTo,
                ResolutionNotes = r.ResolutionNotes,
                ResolvedDate = r.ResolvedDate,
                LastUpdated = r.LastUpdated,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<MaintenanceRequestDto?> GetByIdAsync(int requestId)
    {
        var r = await _context.MaintenanceRequests.FindAsync(requestId);
        return r == null ? null : new MaintenanceRequestDto
        {
            RequestId = r.RequestId,
            UserId = r.UserId,
            PropertyId = r.PropertyId,
            RequestDate = r.RequestDate,
            Category = r.Category,
            Description = r.Description,
            PriorityLevel = r.PriorityLevel,
            Status = r.Status,
            AssignedTo = r.AssignedTo,
            ResolutionNotes = r.ResolutionNotes,
            ResolvedDate = r.ResolvedDate,
            LastUpdated = r.LastUpdated,
            CreatedAt = r.CreatedAt
        };
    }

    public async Task<bool> UpdateAsync(int requestId, MaintenanceRequestDto dto)
    {
        var entity = await _context.MaintenanceRequests.FindAsync(requestId);
        if (entity == null) return false;

        entity.Category = dto.Category;
        entity.Description = dto.Description;
        entity.PriorityLevel = dto.PriorityLevel;
        entity.Status = dto.Status;
        entity.AssignedTo = dto.AssignedTo;
        entity.ResolutionNotes = dto.ResolutionNotes;
        entity.ResolvedDate = dto.ResolvedDate;
        entity.LastUpdated = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int requestId)
    {
        var entity = await _context.MaintenanceRequests.FindAsync(requestId);
        if (entity == null) return false;

        _context.MaintenanceRequests.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<IEnumerable<MaintenanceRequestDto>> GetByUserIdAsync(int userId)
    {
        var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
        if (!userExists)
            throw new InvalidOperationException($"User with ID {userId} does not exist.");

        return await _context.MaintenanceRequests
            .Where(r => r.UserId == userId)
            .Select(r => new MaintenanceRequestDto
            {
                RequestId = r.RequestId,
                UserId = r.UserId,
                PropertyId = r.PropertyId,
                RequestDate = r.RequestDate,
                Category = r.Category,
                Description = r.Description,
                PriorityLevel = r.PriorityLevel,
                Status = r.Status,
                AssignedTo = r.AssignedTo,
                ResolutionNotes = r.ResolutionNotes,
                ResolvedDate = r.ResolvedDate,
                LastUpdated = r.LastUpdated,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<MaintenanceRequestDto>> GetByPropertyIdAsync(int propertyId)
    {
        var propertyExists = await _context.Property.AnyAsync(p => p.PropertyId == propertyId);
        if (!propertyExists)
            throw new InvalidOperationException($"Property with ID {propertyId} does not exist.");

        return await _context.MaintenanceRequests
            .Where(r => r.PropertyId == propertyId)
            .Select(r => new MaintenanceRequestDto
            {
                RequestId = r.RequestId,
                UserId = r.UserId,
                PropertyId = r.PropertyId,
                RequestDate = r.RequestDate,
                Category = r.Category,
                Description = r.Description,
                PriorityLevel = r.PriorityLevel,
                Status = r.Status,
                AssignedTo = r.AssignedTo,
                ResolutionNotes = r.ResolutionNotes,
                ResolvedDate = r.ResolvedDate,
                LastUpdated = r.LastUpdated,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();
    }
}