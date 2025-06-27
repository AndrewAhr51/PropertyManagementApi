using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Pricing
{
    public class PricingDto
    {
        public int PriceId { get; set; }

        [Required]
        public int PropertyId { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal RentalAmount { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal DepositAmount { get; set; }
        
        [Required]
        public string LeaseTerm { get; set; }
        
        public bool UtilitiesIncluded { get; set; } = false;
        public string CreatedBy { get; set; } = "Web"; // Default value for CreatedBy  
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}