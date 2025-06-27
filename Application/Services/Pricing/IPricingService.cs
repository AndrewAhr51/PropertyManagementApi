using PropertyManagementAPI.Domain.DTOs.Property;

public interface IPricingService
{
    Task<PricingDto> CreateAsync(PricingDto dto);
    Task<IEnumerable<PricingDto>> GetAllAsync();
    Task<PricingDto> GetByIdAsync(int id);
    Task<bool> UpdateAsync(int id, PricingDto dto);
    Task<PricingDto> GetLatestForPropertyAsync(int propertyId);

}