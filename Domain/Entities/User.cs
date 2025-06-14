using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities
{
    [Table("Users")]

    public class User
    {
        [Key]
        public int UserId { get; set; }
        [Required, MaxLength(50)]
        public string UserName { get; set; }

        [Required, MaxLength(100)]
        public string Email { get; set; }

        [Required, MaxLength(255)]
        public string PasswordHash { get; set; }

        public int RoleId { get; set; } // Foreign key handled in SQL

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // 🔹 Password Reset Handling
        [MaxLength(255)]
        public string? ResetToken { get; set; }

        public DateTimeOffset? ResetTokenExpiration { get; set; }

        // 🔹 Multi-Factor Authentication (MFA)
        [MaxLength(6)]
        public string? MfaCode { get; set; }

        public DateTimeOffset? MfaCodeExpiration { get; set; }

        public bool IsMfaEnabled { get; set; } = false; // ✅ Tracks MFA status

        public bool IsActive { get; set; } = true;
    }
}