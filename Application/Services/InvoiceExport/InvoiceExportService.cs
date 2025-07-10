using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PropertyManagementAPI.Domain.DTOs.Invoices;
using PropertyManagementAPI.Domain.Entities.Invoices;
using System.Text;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace PropertyManagementAPI.Application.Services.InvoiceExport
{
    public class InvoiceExportService : IInvoiceExportService
    {
        private readonly ILogger<InvoiceExportService> _logger;

        public InvoiceExportService(ILogger<InvoiceExportService> logger)
        {
            _logger = logger;
        }

        public async Task<byte[]> ExportToPdfAsync(IEnumerable<InvoiceDto> invoices)
        {
            try
            {
                using var stream = new MemoryStream();
                var doc = new PdfDocument();
                var page = doc.AddPage();
                var gfx = XGraphics.FromPdfPage(page);
                var font = new XFont("Verdana", 10);

                double y = 20;
                foreach (var invoice in invoices)
                {
                    string line = $" #{invoice.InvoiceId}: {invoice.Amount:C} (Due {invoice.CreatedDate:d})";
                    gfx.DrawString(line, font, XBrushes.Black, new XRect(20, y, page.Width - 40, page.Height), XStringFormats.TopLeft);
                    y += 20;
                }

                doc.Save(stream, false);
                _logger.LogInformation("PDF export succeeded for {Count} invoices.", invoices.Count());
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PDF export failed.");
                throw;
            }
        }

        public async Task<byte[]> ExportToExcelAsync(IEnumerable<InvoiceDto> invoices)
        {
            try
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Invoices");

                worksheet.Cell(1, 1).Value = "InvoiceId";
                worksheet.Cell(1, 2).Value = "PropertyId";
                worksheet.Cell(1, 3).Value = "Amount";
                worksheet.Cell(1, 4).Value = "IssueDate";
                worksheet.Cell(1, 5).Value = "Status";

                int row = 2;
                foreach (var invoice in invoices)
                {
                    worksheet.Cell(row, 1).Value = invoice.InvoiceId;
                    worksheet.Cell(row, 2).Value = invoice.PropertyId;
                    worksheet.Cell(row, 3).Value = invoice.Amount;
                    worksheet.Cell(row, 4).Value = invoice.CreatedDate;
                    worksheet.Cell(row, 5).Value = invoice.Status;
                    row++;
                }

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                _logger.LogInformation("Excel export succeeded for {Count} invoices.", invoices.Count());
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excel export failed.");
                throw;
            }
        }

        public async Task<byte[]> ExportToCsvAsync(IEnumerable<InvoiceDto> invoices)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("Invoice Number,Customer Name,Invoice Date,Due Date,Item Name,Item Amount");

                foreach (var invoice in invoices)
                {
                    sb.AppendLine($"{invoice.InvoiceId},{invoice.TenantName},{invoice.CreatedDate:yyyy-MM-dd},{invoice.DueDate:yyyy-MM-dd},{invoice.Amount}");
                }

                _logger.LogInformation("CSV export succeeded for {Count} invoices.", invoices.Count());
                return Encoding.UTF8.GetBytes(sb.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CSV export failed.");
                throw;
            }
        }

    }
}
           

