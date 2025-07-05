using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Users
{
    public class TenantDto
    {
        public int TenantId { get; set; }

        [Required]
        public int PropertyId { get; set; }

        public string QuickBooksAccessToken { get; set; } = string.Empty;

        public string QuickBooksRefreshToken { get; set; } = string.Empty;

        public string RealmId { get; set; } = string.Empty; // QuickBooks Realm ID, if applicable

        [Required]
        public int UserId { get; set; }
        public bool PrimaryTenant { get; set; } = false;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public decimal Balance { get; set; } = 0.0m;
        public bool IsActive { get; set; } = true; // Default to active
        public DateTime? MoveInDate { get; set; }
        public string CreatedBy { get; set; } = "Web"; // Default value for CreatedBy  
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}