using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities.Payments
{
    public class CreditCardPayment : Payment
    {
        [MaxLength(50)]
        public string CardType { get; set; } = null!;

        [MaxLength(4)]
        public string Last4Digits { get; set; } = null!;

        [MaxLength(50)]
        public string AuthorizationCode { get; set; } = null!;
    }
}