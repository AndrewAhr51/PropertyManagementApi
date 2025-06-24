namespace PropertyManagementAPI.Application.Services.InvoiceExport
{
    public interface IExportService<TDto>
    {
        Task<byte[]> ExportToPdfAsync(IEnumerable<TDto> items);
        Task<byte[]> ExportToExcelAsync(IEnumerable<TDto> items);
    }

}
