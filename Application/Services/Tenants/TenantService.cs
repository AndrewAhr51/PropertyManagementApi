using PropertyManagementAPI.Domain.DTOs.Users;

public class TenantService : ITenantService
{
    private readonly ITenantRepository _repository;

    public TenantService(ITenantRepository repository)
    {
        _repository = repository;
    }

    public async Task<TenantDto> CreateTenantsAsync(TenantDto dto)
    {
        return await _repository.AddTenantAsync(dto);
    }

    public async Task<IEnumerable<TenantDto>> GetAllTenantsAsync()
    {
        return await _repository.GetAllTenantsAsync();
    }

    public async Task<TenantDto?> GetTenantByIdAsync(int tenantId)
    {
        return await _repository.GetTenantByIdAsync(tenantId);
    }

    public async Task<bool> UpdateTenantAsync(int tenantId, TenantDto dto)
    {
        return await _repository.UpdateTenantAsync(tenantId, dto);
    }

    public async Task<bool> SetActivateTenant(int tenantId)
    {
        return await _repository.SetActivateTenant(tenantId);
    }
}