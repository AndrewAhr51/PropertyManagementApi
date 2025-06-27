using PropertyManagementAPI.Domain.DTOs.Property;

public interface ILeaseService
{
    Task<LeaseDto> CreateLeaseAsync(LeaseDto dto);
    Task<IEnumerable<LeaseDto>> GetAllLeaseAsync();
    Task<LeaseDto?> GetLeaseByIdAsync(int leaseId);
    Task<bool> UpdateLeaseAsync(int leaseId, LeaseDto dto);
    Task<bool> DeleteLeaseAsync(int leaseId); // Soft delete
}