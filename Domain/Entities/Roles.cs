using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.Entities
{
    public class Roles
    {
        [Key] // ✅ Explicitly define primary key
        public int RoleId { get; set; }

        public required string Name { get; set; } = string.Empty; // ✅ Defaults to empty string
        public required string Description { get; set; } = string.Empty; // ✅ Defaults to empty string
    }
}
