namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class LeaseTerminationInvoiceCreateDto : InvoiceDto
    {   
        public string TerminationReason { get; set; } = string.Empty;
    }

}
