using PropertyManagementAPI.Domain.DTOs.Property;

public class PricingService : IPricingService
{
    private readonly IPricingRepository _repository;

    public PricingService(IPricingRepository repository)
    {
        _repository = repository;
    }

    public async Task<PricingDto> CreatePricingAsync(PricingDto dto)
    {
        return await _repository.AddPricingAsync(dto);
    }

    public async Task<IEnumerable<PricingDto>> GetAllAsync()
    {
        return await _repository.GetPricingAllPricingAsync();
    }

    public async Task<PricingDto> GetPricingByIdAsync(int id)
    {
        return await _repository.GetPricingByIdAsync(id);
    }

    public async Task<bool> UpdatePricingAsync(PricingDto dto)
    {
        return await _repository.UpdatePricingAsync(dto);
    }

    public async Task<PricingDto> GetLatestForPropertyAsync(int propertyId)
    {
        return await _repository.GetLatestForPropertyAsync(propertyId);
    }
}