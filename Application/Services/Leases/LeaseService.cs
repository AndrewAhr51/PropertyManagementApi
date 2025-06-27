using PropertyManagementAPI.Domain.DTOs.Property;

public class LeaseService : ILeaseService
{
    private readonly ILeaseRepository _repository;

    public LeaseService(ILeaseRepository repository)
    {
        _repository = repository;
    }

    public async Task<LeaseDto> CreateAsync(LeaseDto dto)
    {
        return await _repository.CreateLeaseAsync(dto);
    }

    public async Task<IEnumerable<LeaseDto>> GetAllAsync()
    {
        return await _repository.GetAllLeasesAsync();
    }

    public async Task<LeaseDto?> GetByIdAsync(int leaseId)
    {
        return await _repository.GetLeaseByIdAsync(leaseId);
    }

    public async Task<bool> UpdateAsync(int leaseId, LeaseDto dto)
    {
        return await _repository.UpdateLeaseByIdAsync(leaseId, dto);
    }

    public async Task<bool> DeleteAsync(int leaseId)
    {
        return await _repository.DeleteLeaseByIdAsync(leaseId);
    }
}