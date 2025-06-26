namespace PropertyManagementAPI.Domain.DTOs.Payments
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public int InvoiceId { get; set; }
        public int TenantId { get; set; }
        public int PropertyId { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethodId { get; set; }
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
        public DateTime TransactionDate { get; set; }
        public string ReferenceNumber { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
    }

}
