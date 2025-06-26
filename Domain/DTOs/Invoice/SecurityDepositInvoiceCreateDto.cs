namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class SecurityDepositInvoiceCreateDto : InvoiceCreateDto
    {
        public bool IsRefundable { get; set; }
    }

}
