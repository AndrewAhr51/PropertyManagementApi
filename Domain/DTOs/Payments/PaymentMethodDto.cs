using System.ComponentModel.DataAnnotations;

namespace PropertyManagementAPI.Domain.DTOs.Payments
{
    public class PaymentMethodDto
    {
        public string PreferredMethod { get; set; } // "Card", "Check", "Transfer"
        public string Last4Digits { get; set; }
        public string BankName { get; set; }
        public bool IsDefault { get; set; }
    }
}

