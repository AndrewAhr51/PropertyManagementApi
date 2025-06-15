using PropertyManagementAPI.Domain.DTOs;

public class LeaseService : ILeaseService
{
    private readonly ILeaseRepository _repository;

    public LeaseService(ILeaseRepository repository)
    {
        _repository = repository;
    }

    public async Task<LeaseDto> CreateAsync(LeaseDto dto)
    {
        return await _repository.AddAsync(dto);
    }

    public async Task<IEnumerable<LeaseDto>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<LeaseDto?> GetByIdAsync(int leaseId)
    {
        return await _repository.GetByIdAsync(leaseId);
    }

    public async Task<bool> UpdateAsync(int leaseId, LeaseDto dto)
    {
        return await _repository.UpdateAsync(leaseId, dto);
    }

    public async Task<bool> DeleteAsync(int leaseId)
    {
        return await _repository.DeleteAsync(leaseId);
    }
}