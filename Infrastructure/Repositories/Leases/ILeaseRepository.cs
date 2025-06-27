using PropertyManagementAPI.Domain.DTOs.Property;

public interface ILeaseRepository
{
    Task<LeaseDto> CreateLeaseAsync(LeaseDto dto);
    Task<IEnumerable<LeaseDto>> GetAllLeasesAsync();
    Task<LeaseDto?> GetLeaseByIdAsync(int leaseId);
    Task<bool> UpdateLeaseByIdAsync(int leaseId, LeaseDto dto);
    Task<bool> DeleteLeaseByIdAsync(int leaseId); // Soft delete
}