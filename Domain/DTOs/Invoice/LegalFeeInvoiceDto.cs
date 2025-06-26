namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class LegalFeeInvoiceDto
    {
        public int InvoiceId { get; set; }
        public string CaseReference { get; set; } = null!;
        public string LawFirm { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string? Notes { get; set; }
    }
}
