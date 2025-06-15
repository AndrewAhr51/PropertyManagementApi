using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs
{
    public class LeaseDto
    {
        public int LeaseId { get; set; }

        [Required]
        public int TenantId { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public decimal MonthlyRent { get; set; }

        public bool DepositPaid { get; set; }

        public bool IsActive { get; set; }

        public DateTime SignedDate { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}