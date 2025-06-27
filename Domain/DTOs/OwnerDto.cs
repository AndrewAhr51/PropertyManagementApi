using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs
{
    public class OwnerDto
    {
        public int OwnerId { get; set; } 

        [Required]
        public int UserId { get; set; }

        [Required]
        public bool PrimaryOwner { get; set; } = false;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(50)]
        public string Phone { get; set; }

        [MaxLength(255)]
        public string Address1 { get; set; }

        [MaxLength(255)]
        public string Address2 { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string State { get; set; }

        [MaxLength(20)]
        public string PostalCode { get; set; }

        [MaxLength(100)]
        public string Country { get; set; }

        public bool IsActive { get; set; } = true;
    }
}