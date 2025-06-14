using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs
{
    public class PermissionsDto
    {
        public int PermissionId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty; // ✅ Defaults to empty string

        [StringLength(255)]
        public string Description { get; set; } = string.Empty; // ✅ Defaults to empty string
    }
}
