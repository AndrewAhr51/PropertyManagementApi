using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.Entities;

public class CreditCardInfo
{
    [Key]  // Explicitly mark as primary key
    public int CardId { get; set; }
    public int TenantId { get; set; }
    public int PropertyId { get; set; }
    public string CardHolderName { get; set; }
    public byte [] CardNumber { get; set; }  // Will be encrypted
    public string LastFourDigits { get; set; } // Extracted from CardNumber
    public string ExpirationDate { get; set; }
    public byte[] CVV { get; set; }  // Encrypted CVV
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}