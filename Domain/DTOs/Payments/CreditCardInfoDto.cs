using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Payments
{
    public class CreditCardInfoDto
    {
        public int CardId { get; set; }
        public int TenantId { get; set; }
        public int PropertyId { get; set; }
        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }  // Will be encrypted
        public string LastFourDigits { get; set; }
        public string ExpirationDate { get; set; }
        public string CVV { get; set; }  // Storing CVV as an integer
        public string CreatedBy { get; set; } = "Web"; // Default value for CreatedBy  
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
