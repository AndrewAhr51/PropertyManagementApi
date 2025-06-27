using PropertyManagementAPI.Domain.DTOs.Maintenance;

public interface IMaintenanceRequestRepository
{
    Task<MaintenanceRequestDto> AddMaintenanceRequestAsync(MaintenanceRequestDto dto);
    Task<IEnumerable<MaintenanceRequestDto>> GetAllMaintenanceRequestAsync();
    Task<MaintenanceRequestDto?> GetMaintenanceRequestByIdAsync(int requestId);
    Task<bool> UpdateMaintenanceRequestAsync(int requestId, MaintenanceRequestDto dto);
    Task<bool> DeleteMaintenanceRequestAsync(int requestId);
    Task<IEnumerable<MaintenanceRequestDto>> GetMaintenanceRequestByUserIdAsync(int userId);
    Task<IEnumerable<MaintenanceRequestDto>> GetMaintenanceRequestByPropertyIdAsync(int propertyId);
}