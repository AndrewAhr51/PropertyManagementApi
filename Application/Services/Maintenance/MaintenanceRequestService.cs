using PropertyManagementAPI.Domain.DTOs.Maintenance;

public class MaintenanceRequestService : IMaintenanceRequestService
{
    private readonly IMaintenanceRequestRepository _repository;

    public MaintenanceRequestService(IMaintenanceRequestRepository repository)
    {
        _repository = repository;
    }

    public async Task<MaintenanceRequestDto> CreateMaintenanceRequestAsync(MaintenanceRequestDto dto)
    {
        return await _repository.AddMaintenanceRequestAsync(dto);
    }

    public async Task<IEnumerable<MaintenanceRequestDto>> GetAllMaintenanceRequestAsync()
    {
        return await _repository.GetAllMaintenanceRequestAsync();
    }

    public async Task<MaintenanceRequestDto?> GetMaintenanceRequestByIdAsync(int requestId)
    {
        return await _repository.GetMaintenanceRequestByIdAsync(requestId);
    }

    public async Task<bool> UpdateMaintenanceRequestAsync(int requestId, MaintenanceRequestDto dto)
    {
        return await _repository.UpdateMaintenanceRequestAsync(requestId, dto);
    }

    public async Task<bool> DeleteMaintenanceRequestAsync(int requestId)
    {
        return await _repository.DeleteMaintenanceRequestAsync(requestId);
    }
    public async Task<IEnumerable<MaintenanceRequestDto>> GetMaintenanceRequestByUserIdAsync(int userId)
    {
        return await _repository.GetMaintenanceRequestByUserIdAsync(userId);
    }

    public async Task<IEnumerable<MaintenanceRequestDto>> GetMaintenanceRequestByPropertyIdAsync(int propertyId)
    {
        return await _repository.GetMaintenanceRequestByPropertyIdAsync(propertyId);
    }
}