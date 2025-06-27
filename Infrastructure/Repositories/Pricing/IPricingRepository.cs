using PropertyManagementAPI.Domain.DTOs.Property;

public interface IPricingRepository
{
    Task<PricingDto> AddAsync(PricingDto dto);
    Task<IEnumerable<PricingDto>> GetAllAsync();
    Task<PricingDto> GetByIdAsync(int id);
    Task<bool> UpdateAsync(int id, PricingDto dto);
    Task<PricingDto> GetLatestForPropertyAsync(int id);
}