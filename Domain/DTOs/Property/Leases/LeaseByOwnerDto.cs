using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Property.Leases
{
    public class LeaseByOwnerDto
    {
        public int LeaseId { get; set; }
        public int TenantId { get; set; }
        public string TenantFirstName { get; set; } = string.Empty; // Default value for TenantName
        public string TenantLastName { get; set; } = string.Empty; // Default value for TenantName
        public string TenantPhoneNumber { get; set; } = string.Empty; // Default value for TenantName
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public int Discount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal DepositAmount { get; set; }
        public bool DepositPaid { get; set; }
        public bool IsActive { get; set; }
        public DateTime SignedDate { get; set; }
        public string CreatedBy { get; set; } = "Web"; // Default value for CreatedBy  
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}