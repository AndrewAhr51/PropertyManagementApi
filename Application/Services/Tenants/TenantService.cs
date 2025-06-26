using PropertyManagementAPI.Domain.DTOs;

public class TenantService : ITenantService
{
    private readonly ITenantRepository _repository;

    public TenantService(ITenantRepository repository)
    {
        _repository = repository;
    }

    public async Task<TenantDto> CreateAsync(TenantDto dto)
    {
        return await _repository.AddAsync(dto);
    }

    public async Task<IEnumerable<TenantDto>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<TenantDto?> GetByIdAsync(int tenantId)
    {
        return await _repository.GetByIdAsync(tenantId);
    }

    public async Task<bool> UpdateAsync(int tenantId, TenantDto dto)
    {
        return await _repository.UpdateAsync(tenantId, dto);
    }

    public async Task<bool> DeleteAsync(int tenantId)
    {
        return await _repository.DeleteAsync(tenantId);
    }
}