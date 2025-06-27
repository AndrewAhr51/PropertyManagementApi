using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PropertyManagementAPI.Domain.DTOs.Maintenance;
using PropertyManagementAPI.Domain.Entities.Maintenance;
using PropertyManagementAPI.Infrastructure.Data;

public class MaintenanceRequestRepository : IMaintenanceRequestRepository
{
    private readonly MySqlDbContext _context;

    public MaintenanceRequestRepository(MySqlDbContext context)
    {
        _context = context;
    }

    public async Task<MaintenanceRequestDto> AddMaintenanceRequestAsync(MaintenanceRequestDto maintenanceRequestDto)
    {
        var userExists = await _context.Users.AnyAsync(u => u.UserId == maintenanceRequestDto.UserId);
        if (!userExists)
            throw new InvalidOperationException($"User with ID {maintenanceRequestDto.UserId} does not exist.");

        var propertyExists = await _context.Properties.AnyAsync(p => p.PropertyId == maintenanceRequestDto.PropertyId);
        if (!propertyExists)
            throw new InvalidOperationException($"Property with ID { maintenanceRequestDto.PropertyId} does not exist.");

        var entity = new MaintenanceRequests
        {
            UserId = maintenanceRequestDto.UserId,
            PropertyId = maintenanceRequestDto.PropertyId,
            RequestDate = maintenanceRequestDto.RequestDate == default ? DateTime.UtcNow : maintenanceRequestDto.RequestDate,
            Category = maintenanceRequestDto.Category,
            Description = maintenanceRequestDto.Description,
            PriorityLevel = maintenanceRequestDto.PriorityLevel ?? "Normal",
            Status = "Open",
            AssignedTo = string.Empty,
            ResolutionNotes = string.Empty,
            ResolvedDate = null,
        };

        _context.MaintenanceRequests.Add(entity);
        await _context.SaveChangesAsync();

        maintenanceRequestDto.RequestId = entity.RequestId;
        maintenanceRequestDto.CreatedBy = entity.CreatedBy;
        return maintenanceRequestDto;
    }

    public async Task<IEnumerable<MaintenanceRequestDto>> GetAllMaintenanceRequestAsync()
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
                CreatedBy = r.CreatedBy
            })
            .ToListAsync();
    }

    public async Task<MaintenanceRequestDto?> GetMaintenanceRequestByIdAsync(int requestId)
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
            CreatedBy = r.CreatedBy
        };
    }

    public async Task<bool> UpdateMaintenanceRequestAsync(int requestId, MaintenanceRequestDto dto)
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

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteMaintenanceRequestAsync(int requestId)
    {
        var entity = await _context.MaintenanceRequests.FindAsync(requestId);
        if (entity == null) return false;

        _context.MaintenanceRequests.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<IEnumerable<MaintenanceRequestDto>> GetMaintenanceRequestByUserIdAsync(int userId)
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
                CreatedBy = r.CreatedBy
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<MaintenanceRequestDto>> GetMaintenanceRequestByPropertyIdAsync(int propertyId)
    {
        var propertyExists = await _context.Properties.AnyAsync(p => p.PropertyId == propertyId);
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
                CreatedBy = r.CreatedBy
            })
            .ToListAsync();
    }
}