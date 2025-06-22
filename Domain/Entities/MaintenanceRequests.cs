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

        public string Description { get; set; } = string.Empty;

        [MaxLength(50)]
        public string PriorityLevel { get; set; } = "Normal";

        [MaxLength(50)]
        public string Status { get; set; } = "Open";

        [MaxLength(100)]
        public string AssignedTo { get; set; } = string.Empty;

        public string ResolutionNotes { get; set; } = string.Empty;

        public DateTime? ResolvedDate { get; set; } = null;

        public string CreatedBy { get; set; } = "Web"; // Default value for CreatedBy  
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}