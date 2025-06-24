using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using System.Text;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace PropertyManagementAPI.Application.Services.InvoiceExport
{
    public class InvoiceExportService : IInvoiceExportService
    {
        public async Task<byte[]> ExportToPdfAsync(IEnumerable<CumulativeInvoiceDto> invoices)
        {
            using var stream = new MemoryStream();
            var doc = new PdfDocument();
            var page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Verdana", 10);

            double y = 20;
            foreach (var invoice in invoices)
            {
                string line = $"[{invoice.InvoiceType}] #{invoice.InvoiceId}: {invoice.Amount:C} (Due {invoice.CreatedDate:d})";
                gfx.DrawString(line, font, XBrushes.Black, new XRect(20, y, page.Width - 40, page.Height), XStringFormats.TopLeft);
                y += 20;
            }

            doc.Save(stream, false);
            return stream.ToArray();
        }

        public async Task<byte[]> ExportToExcelAsync(IEnumerable<CumulativeInvoiceDto> invoices)
        {
            using var workbook = new XLWorkbook(); // Ensure ClosedXML.Excel is referenced in your project
            var worksheet = workbook.Worksheets.Add("Invoices");

            worksheet.Cell(1, 1).Value = "InvoiceId";
            worksheet.Cell(1, 2).Value = "PropertyId";
            worksheet.Cell(1, 3).Value = "Amount";
            worksheet.Cell(1, 4).Value = "IssueDate";
            worksheet.Cell(1, 5).Value = "Status";
            worksheet.Cell(1, 6).Value = "InvoiceType";

            int row = 2;
            foreach (var invoice in invoices)
            {
                worksheet.Cell(row, 1).Value = invoice.InvoiceId;
                worksheet.Cell(row, 2).Value = invoice.PropertyId;
                worksheet.Cell(row, 3).Value = invoice.Amount;
                worksheet.Cell(row, 4).Value = invoice.CreatedDate;
                worksheet.Cell(row, 5).Value = invoice.Status;
                worksheet.Cell(row, 6).Value = invoice.InvoiceType;
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> ExportToCsvAsync(IEnumerable<CumulativeInvoiceDto> invoices)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Invoice Number,Customer Name,Invoice Date,Due Date,Item Name,Item Amount");

            foreach (var invoice in invoices)
            {
                sb.AppendLine($"{invoice.InvoiceId},{invoice.CustomerName},{invoice.CreatedDate:yyyy-MM-dd},{invoice.DueDate:yyyy-MM-dd},{invoice.InvoiceType},{invoice.Amount}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

    }
}
           

