﻿namespace PropertyManagementAPI.Domain.DTOs.Roles
{
    public class CreateRoleDto
    {
        public required string Name { get; set; } = string.Empty; // ✅ Defaults to empty string
        public required string Description { get; set; } = string.Empty; // ✅ Defaults to empty string
    }
}
