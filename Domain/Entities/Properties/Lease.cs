using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities.Properties
{
    public class Lease
    {
        [Key]
        public int LeaseId { get; set; }

        [Required]
        public int TenantId { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required]
        public int Discount { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal MonthlyRent { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal DepositAmount { get; set; }

        public bool DepositPaid { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime SignedDate { get; set; } = DateTime.UtcNow;

        public string CreatedBy { get; set; } = "Web"; // Default value for CreatedBy  
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}