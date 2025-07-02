using PropertyManagementAPI.Domain.Entities.User;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities.Payments.CreditCard
{
    public class CardToken
    {
        [Key]
        public int CardTokenId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TokenValue { get; set; }

        [MaxLength(20)]
        public string CardBrand { get; set; }

        [MaxLength(4)]
        public string Last4Digits { get; set; }

        [Required]
        public DateTime Expiration { get; set; }

        public int TenantId { get; set; }
        public int OwnerId { get; set; }

        public bool IsDefault { get; set; } = false;

        public DateTime LinkedOn { get; set; } = DateTime.UtcNow;

        // 🔗 Navigation properties
        public Tenant Tenant { get; set; }
        public Owner Owner { get; set; }
    }
}