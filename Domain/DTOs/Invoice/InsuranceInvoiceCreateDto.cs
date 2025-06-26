namespace PropertyManagementAPI.Domain.DTOs.Invoice
{
    public class InsuranceInvoiceCreateDto: InvoiceCreateDto
    {   
        public string PolicyNumber { get; set; } = string.Empty;
        public DateTime CoveragePeriodStart { get; set; }
        public DateTime CoveragePeriodEnd { get; set; }
    }

}
