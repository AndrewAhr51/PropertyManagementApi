using PropertyManagementAPI.Domain.DTOs.Property;

public class LeaseService : ILeaseService
{
    private readonly ILeaseRepository _repository;

    public LeaseService(ILeaseRepository repository)
    {
        _repository = repository;
    }

    public async Task<LeaseDto> CreateLeaseAsync(LeaseDto dto)
    {
        return await _repository.CreateLeaseAsync(dto);
    }

    public async Task<IEnumerable<LeaseDto>> GetAllLeaseAsync()
    {
        return await _repository.GetAllLeasesAsync();
    }

    public async Task<LeaseDto?> GetLeaseByIdAsync(int leaseId)
    {
        return await _repository.GetLeaseByIdAsync(leaseId);
    }

    public async Task<bool> UpdateLeaseAsync(int leaseId, LeaseDto dto)
    {
        return await _repository.UpdateLeaseByIdAsync(leaseId, dto);
    }

    public async Task<bool> DeleteLeaseAsync(int leaseId)
    {
        return await _repository.DeleteLeaseByIdAsync(leaseId);
    }
}