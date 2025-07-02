using PropertyManagementAPI.Domain.DTOs.Property.Leases;

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

    public async Task<bool> UpdateLeaseAsync(LeaseUpdateDto dto)
    {
        return await _repository.UpdateLeaseByIdAsync(dto);
    }

    public async Task<bool> DeleteLeaseAsync(int leaseId)
    {
        return await _repository.DeleteLeaseByIdAsync(leaseId);
    }

    public async Task<IEnumerable<LeaseDto>> GetAllLeasesByOwnerIdAsync(int ownerId)
    {
        return await _repository.GetAllLeasesByOwnerIdAsync(ownerId);
    }

    public async Task<LeaseDto> GetLeaseByTenantIdAsync(int tenantId)
    {
        return await _repository.GetLeaseByTenantIdAsync(tenantId);
    }
}