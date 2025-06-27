using PropertyManagementAPI.Domain.DTOs.Maintenance;

public interface IMaintenanceRequestRepository
{
    Task<MaintenanceRequestDto> AddAsync(MaintenanceRequestDto dto);
    Task<IEnumerable<MaintenanceRequestDto>> GetAllAsync();
    Task<MaintenanceRequestDto?> GetByIdAsync(int requestId);
    Task<bool> UpdateAsync(int requestId, MaintenanceRequestDto dto);
    Task<bool> DeleteAsync(int requestId);
    Task<IEnumerable<MaintenanceRequestDto>> GetByUserIdAsync(int userId);
    Task<IEnumerable<MaintenanceRequestDto>> GetByPropertyIdAsync(int propertyId);
}