using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Maintenance
{
    public class MaintenanceRequestDto
    {
        public int RequestId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int PropertyId { get; set; }

        public DateTime RequestDate { get; set; }

        public string Category { get; set; }

        public string Description { get; set; } = string.Empty;

        public string PriorityLevel { get; set; } = "Low";

        public string Status { get; set; } = string.Empty;

        public string AssignedTo { get; set; } = string.Empty;

        public string ResolutionNotes { get; set; } = string.Empty;

        public DateTime? ResolvedDate { get; set; } = null;

        public string CreatedBy { get; set; } = "Web"; // Default value for CreatedBy  
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}