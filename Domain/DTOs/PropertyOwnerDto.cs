
namespace PropertyManagementAPI.Domain.DTOs
{
    public class PropertyOwnerDto
    {
        public int PropertyId { get; set; }
        public int OwnerId { get; set; }
        public decimal OwnershipPercentage { get; set; } = 100;
    }
}