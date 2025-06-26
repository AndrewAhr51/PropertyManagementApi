namespace PropertyManagementAPI.Domain.DTOs.Payments
{
    public class CreatePaymentDto
    {
        public int InvoiceId { get; set; }
        public int TenantId { get; set; }
        public int PropertyId { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethodId { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Notes { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = null!;
        public string CreatedBy { get; set; } = "Web";
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }

}
