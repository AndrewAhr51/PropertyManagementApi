using PropertyManagementAPI.Domain.DTOs;

public interface ILeaseRepository
{
    Task<LeaseDto> AddAsync(LeaseDto dto);
    Task<IEnumerable<LeaseDto>> GetAllAsync();
    Task<LeaseDto?> GetByIdAsync(int leaseId);
    Task<bool> UpdateAsync(int leaseId, LeaseDto dto);
    Task<bool> DeleteAsync(int leaseId); // Soft delete
}