using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementAPI.Domain.Entities.Payments
{
    public class WireTransfer : Payment
    {
        [MaxLength(50)]
        public string BankAccountNumber { get; set; } = null!;

        [MaxLength(20)]
        public string RoutingNumber { get; set; } = null!;

        [MaxLength(50)]
        public string TransactionId { get; set; } = null!;
    }
}