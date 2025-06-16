namespace PropertyManagementAPI.Domain.DTOs
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
        public DateTime CreatedAt { get; set; }
    }
}
