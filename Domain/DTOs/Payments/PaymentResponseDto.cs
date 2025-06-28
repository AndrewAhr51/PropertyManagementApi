namespace PropertyManagementAPI.Domain.DTOs.Payments
{
    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidOn { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNumber { get; set; }

        public int InvoiceId { get; set; }
        public string InvoiceNumber { get; set; }

        public int TenantId { get; set; }
        public string TenantName { get; set; }

        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
    }

}
