using PropertyManagementAPI.Domain.DTOs.Invoice;

namespace PropertyManagementAPI.Application.Services.InvoiceExport
{
    public interface IInvoiceExportService: IExportService<CumulativeInvoiceDto>

    {
       public Task<byte[]> ExportToPdfAsync(IEnumerable<CumulativeInvoiceDto> invoices);
       public Task<byte[]> ExportToExcelAsync(IEnumerable<CumulativeInvoiceDto> invoices);
       public Task<byte[]> ExportToCsvAsync(IEnumerable<CumulativeInvoiceDto> invoices);
    }
}
