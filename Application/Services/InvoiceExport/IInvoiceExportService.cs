using PropertyManagementAPI.Domain.DTOs.Invoices;

namespace PropertyManagementAPI.Application.Services.InvoiceExport
{
    public interface IInvoiceExportService: IExportService<InvoiceDto>

    {
       public Task<byte[]> ExportToPdfAsync(IEnumerable<InvoiceDto> invoices);
       public Task<byte[]> ExportToExcelAsync(IEnumerable<InvoiceDto> invoices);
       public Task<byte[]> ExportToCsvAsync(IEnumerable<InvoiceDto> invoices);   
    }
}
