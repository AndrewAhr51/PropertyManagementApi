using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Roles
{
    public class RolePermissionDto
    {
        [Required]
        public int RoleId { get; set; }

        [Required]
        public int PermissionId { get; set; }
    }
}
