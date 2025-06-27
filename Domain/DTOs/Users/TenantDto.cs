using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Users
{
    public class TenantDto
    {
        public int TenantId { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required]
        public int UserId { get; set; }
        public bool PrimaryTenant { get; set; } = false;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime? MoveInDate { get; set; }
        public string CreatedBy { get; set; } = "Web"; // Default value for CreatedBy  
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}