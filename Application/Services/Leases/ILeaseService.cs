using PropertyManagementAPI.Domain.DTOs.Property;

public interface ILeaseService
{
    Task<LeaseDto> CreateAsync(LeaseDto dto);
    Task<IEnumerable<LeaseDto>> GetAllAsync();
    Task<LeaseDto?> GetByIdAsync(int leaseId);
    Task<bool> UpdateAsync(int leaseId, LeaseDto dto);
    Task<bool> DeleteAsync(int leaseId); // Soft delete
}