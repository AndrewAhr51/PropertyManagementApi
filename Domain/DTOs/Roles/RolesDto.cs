namespace PropertyManagementAPI.Domain.DTOs.Roles
{
    public class RolesDto
    {
        public required int RoleId { get; set; }

        public required string Name { get; set; } = string.Empty; // ✅ Defaults to empty string

        public required string Description { get; set; } = string.Empty; // ✅ Defaults to empty string
    }
}
