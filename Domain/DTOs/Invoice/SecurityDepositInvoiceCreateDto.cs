namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class SecurityDepositInvoiceCreateDto : InvoiceDto
    {
        public bool IsRefundable { get; set; }
    }

}
