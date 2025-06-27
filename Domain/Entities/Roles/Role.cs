using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.Entities.Roles
{
    public class Role
    {
        [Key] // ✅ Explicitly define primary key
        public int RoleId { get; set; }
        public required string Name { get; set; } = string.Empty; // ✅ Defaults to empty string
        public required string Description { get; set; } = string.Empty; // ✅ Defaults to empty string
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // ✅ Defaults to current UTC time  
        public string CreatedBy { get; set; } = "Web";
    }
}
