using PropertyManagementAPI.Domain.DTOs;

public class PricingService : IPricingService
{
    private readonly IPricingRepository _repository;

    public PricingService(IPricingRepository repository)
    {
        _repository = repository;
    }

    public async Task<PricingDto> CreateAsync(PricingDto dto)
    {
        return await _repository.AddAsync(dto);
    }

    public async Task<IEnumerable<PricingDto>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<PricingDto> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<bool> UpdateAsync(int id, PricingDto dto)
    {
        return await _repository.UpdateAsync(id, dto);
    }

    public async Task<PricingDto> GetLatestForPropertyAsync(int propertyId)
    {
        return await _repository.GetLatestForPropertyAsync(propertyId);
    }
}