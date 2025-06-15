using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs
{
    public class TenantDto
    {
        public int TenantId { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required]
        public int UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime? MoveInDate { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}