namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class SecurityDepositInvoiceCreateDto
    {
        public int InvoiceId { get; set; }
        public bool IsRefundable { get; set; }
    }

}
