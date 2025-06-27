using PropertyManagementAPI.Domain.DTOs.Property;

public interface IPricingService
{
    Task<PricingDto> CreatePricingAsync(PricingDto dto);
    Task<IEnumerable<PricingDto>> GetAllAsync();
    Task<PricingDto> GetPricingByIdAsync(int id);
    Task<bool> UpdatePricingAsync(PricingDto dto);
    Task<PricingDto> GetLatestForPropertyAsync(int propertyId);

}