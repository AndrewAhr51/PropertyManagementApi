using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Property
{
    public class LeaseDto
    {
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
        [Required]
        public decimal MonthlyRent { get; set; }
        public decimal DepositAmount { get; set; }
        public bool DepositPaid { get; set; }
        public bool IsActive { get; set; }
        public DateTime SignedDate { get; set; }
        public string CreatedBy { get; set; } = "Web"; // Default value for CreatedBy  
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}