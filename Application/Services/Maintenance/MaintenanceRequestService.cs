using PropertyManagementAPI.Domain.DTOs.Maintenance;

public class MaintenanceRequestService : IMaintenanceRequestService
{
    private readonly IMaintenanceRequestRepository _repository;

    public MaintenanceRequestService(IMaintenanceRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<MaintenanceRequestDto> CreateAsync(MaintenanceRequestDto dto)
    {
        return await _repository.AddAsync(dto);
    }

    public async Task<IEnumerable<MaintenanceRequestDto>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<MaintenanceRequestDto?> GetByIdAsync(int requestId)
    {
        return await _repository.GetByIdAsync(requestId);
    }

    public async Task<bool> UpdateAsync(int requestId, MaintenanceRequestDto dto)
    {
        return await _repository.UpdateAsync(requestId, dto);
    }

    public async Task<bool> DeleteAsync(int requestId)
    {
        return await _repository.DeleteAsync(requestId);
    }
    public async Task<IEnumerable<MaintenanceRequestDto>> GetByUserIdAsync(int userId)
    {
        return await _repository.GetByUserIdAsync(userId);
    }

    public async Task<IEnumerable<MaintenanceRequestDto>> GetByPropertyIdAsync(int propertyId)
    {
        return await _repository.GetByPropertyIdAsync(propertyId);
    }
}