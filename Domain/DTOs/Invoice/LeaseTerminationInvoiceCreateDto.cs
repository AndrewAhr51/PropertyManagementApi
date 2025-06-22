namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class LeaseTerminationInvoiceCreateDto
    {
        public int InvoiceId { get; set; }
        public string TerminationReason { get; set; } = string.Empty;
    }

}
