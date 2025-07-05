using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Property.Leases
{
    public class LeaseUpdateDto
    {
        public int LeaseId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Amount { get; set; }
        public decimal DepositAmount { get; set; }
        public bool DepositPaid { get; set; }
        public bool IsActive { get; set; }
        public DateTime SignedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}