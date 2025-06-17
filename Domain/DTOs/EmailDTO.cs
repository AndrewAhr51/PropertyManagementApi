using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs
{
    public class EmailDto
    {
        [Required]
        [EmailAddress] // ✅ Ensures valid email format
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Body { get; set; } = string.Empty;

        public int? SenderId { get; set; } // ✅ Optional sender tracking
    }
}