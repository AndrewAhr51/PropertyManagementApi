using PropertyManagementAPI.Domain.DTOs.Property;

public interface IPricingRepository
{
    Task<PricingDto> AddPricingAsync(PricingDto dto);
    Task<IEnumerable<PricingDto>> GetPricingAllPricingAsync();
    Task<PricingDto> GetPricingByIdAsync(int id);
    Task<bool> UpdatePricingAsync(PricingDto dto);
    Task<PricingDto> GetLatestForPropertyAsync(int id);
}