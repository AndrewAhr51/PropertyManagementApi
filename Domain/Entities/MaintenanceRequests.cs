using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities
{
    public class MaintenanceRequests
    {
        [Key]
        public int RequestId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int PropertyId { get; set; }

        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        public string Category { get; set; }

        [Required]
        public string Description { get; set; }

        [MaxLength(50)]
        public string PriorityLevel { get; set; } = "Normal";

        [MaxLength(50)]
        public string Status { get; set; } = "Open";

        [MaxLength(100)]
        public string AssignedTo { get; set; }

        public string ResolutionNotes { get; set; }

        public DateTime? ResolvedDate { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}