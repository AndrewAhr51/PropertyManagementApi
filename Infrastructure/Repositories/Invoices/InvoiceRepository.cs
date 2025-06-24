using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PropertyManagementAPI.Application.Services;
using PropertyManagementAPI.Application.Services.InvoiceExport;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Data;
using System.Linq;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly MySqlDbContext _context;
        private readonly ILogger<InvoiceDto> _logger;
        private readonly IExportService<CumulativeInvoiceDto> _exportService;
        private readonly IEmailService _emailService;



        public InvoiceRepository(MySqlDbContext context, ILogger<InvoiceDto> logger, IExportService<CumulativeInvoiceDto> exportService, IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _exportService = exportService;
            _emailService = emailService;
        }

        public async Task<decimal> GetAmountDueAsync(InvoiceDto invoice, string? UtilityType)
        {
            try
            {
                if (invoice == null)
                {
                    _logger.LogWarning("Invoice is null");
                    return 0;
                }

                if (!await _context.Property.AnyAsync(t => t.PropertyId == invoice.PropertyId))
                {
                    _logger.LogWarning("PropertyId {PropertyId} not found", invoice.PropertyId);
                    return 0;
                }

                var previousMonth = new DateTime(invoice.DueDate.Year, invoice.DueDate.Month, 1).AddMonths(-1);
                decimal amountDue = 0;
                Invoice? previousInvoice = null;

                switch (invoice.InvoiceType)
                {
                    case "Rent":
                        {
                            int invoiceTypeId = await GetInvoiceTypeIdByNameAsync("Rent");

                            var lease = await GetLeaseInformationAsync(invoice.PropertyId);
                            if (lease == null)
                            {
                                _logger.LogWarning("No active lease found for PropertyId {PropertyId}", invoice.PropertyId);
                                return 0;
                            }

                            decimal discount = lease.MonthlyRent * (lease.Discount / 100m);
                            amountDue = lease.MonthlyRent - discount;

                            previousInvoice = await _context.RentInvoices
                                .Where(r => r.InvoiceTypeId == invoiceTypeId &&
                                            r.PropertyId == invoice.PropertyId &&
                                            r.Status != "Paid" &&
                                            r.DueDate.Month == previousMonth.Month &&
                                            r.DueDate.Year == previousMonth.Year)
                                .FirstOrDefaultAsync();
                            break;
                        }

                    case "Utilities":
                        {
                            int invoiceTypeId = await GetInvoiceTypeIdByNameAsync("Utilities");
                            int utilityTypeId = await GetUtilityTypeNameByIdAsync(UtilityType);

                            previousInvoice = await _context.UtilityInvoices
                                .Where(r => r.InvoiceTypeId == invoiceTypeId &&
                                            r.UtilityTypeId == utilityTypeId &&
                                            r.PropertyId == invoice.PropertyId &&
                                            r.Status != "Paid" &&
                                            r.DueDate.Month == previousMonth.Month &&
                                            r.DueDate.Year == previousMonth.Year)
                                .FirstOrDefaultAsync();
                            break;
                        }
                    case "SecurityDeposit":
                        {
                            int invoiceTypeId = await GetInvoiceTypeIdByNameAsync("SecurityDeposit");

                            var lease = await GetLeaseInformationAsync(invoice.PropertyId);
                            if (lease == null)
                            {
                                _logger.LogWarning("No active lease found for PropertyId {PropertyId}", invoice.PropertyId);
                                return 0;
                            }
                            amountDue = lease.DepositAmount;

                            previousInvoice = await _context.SecurityDepositInvoices
                                .Where(r => r.InvoiceTypeId == invoiceTypeId &&
                                            r.PropertyId == invoice.PropertyId &&
                                            r.Status != "Paid" &&
                                            r.DueDate.Month == previousMonth.Month &&
                                            r.DueDate.Year == previousMonth.Year)
                                .FirstOrDefaultAsync();
                            break;
                        }
                    case "PropertyTax":
                        {
                            int invoiceTypeId = await GetInvoiceTypeIdByNameAsync("PropertyTax");

                            var property = await GetPropertyInformationAsync(invoice.PropertyId);
                            if (property == null)
                            {
                                _logger.LogWarning("No active lease found for PropertyId {PropertyId}", invoice.PropertyId);
                                return 0;
                            }
                            amountDue = property.PropertyTaxes;
                            previousInvoice = await _context.PropertyTaxInvoices
                                .Where(r => r.InvoiceTypeId == invoiceTypeId &&
                                            r.PropertyId == invoice.PropertyId &&
                                            r.Status != "Paid" &&
                                            r.DueDate.Month == previousMonth.Month &&
                                            r.DueDate.Year == previousMonth.Year)
                                .FirstOrDefaultAsync();
                            break;

                        }
                    case "Insurance":
                        {
                            int invoiceTypeId = await GetInvoiceTypeIdByNameAsync("Insurance");

                            var property = await GetPropertyInformationAsync(invoice.PropertyId);

                            if (property == null)
                            {
                                _logger.LogWarning("No property found for PropertyId {PropertyId}", invoice.PropertyId);
                                return 0;
                            }
                            amountDue = property.Insurance;
                            previousInvoice = await _context.PropertyTaxInvoices
                                .Where(r => r.InvoiceTypeId == invoiceTypeId &&
                                            r.PropertyId == invoice.PropertyId &&
                                            r.Status != "Paid" &&
                                            r.DueDate.Month == previousMonth.Month &&
                                            r.DueDate.Year == previousMonth.Year)
                                .FirstOrDefaultAsync();
                            break;

                        }
                    default:
                        _logger.LogWarning("Invalid InvoiceType {InvoiceType}", invoice.InvoiceType);
                        return 0;
                }

                if (previousInvoice != null)
                {
                    amountDue += previousInvoice.Amount;
                }

                return amountDue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating amount due for PropertyId {PropertyId}", invoice?.PropertyId);
                return 0;
            }
        }
        public async Task<Lease?> GetLeaseInformationAsync(int propertyId)
        {
            try
            {
                return await _context.Leases
                    .AsNoTracking()
                    .Where(l => l.PropertyId == propertyId && l.IsActive == true)
                    .OrderByDescending(l => l.StartDate)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lease for propertyId {propertyId}", propertyId);
                return null; // Fix for CS8603: Return null explicitly for nullable type.
            }
        }
        public async Task<Properties?> GetPropertyInformationAsync(int propertyId)
        {
            try
            {
                return await _context.Property
                    .AsNoTracking()
                    .Where(p => p.PropertyId == propertyId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving ptoperty information for propertyId {propertyId}", propertyId);
                return null; // Fix for CS8603: Return null explicitly for nullable type.
            }
        }
        public async Task<string> GetPropertyOwnerNameAsync(int propertyId)
        {
            var owner = await _context.PropertyOwners
                .Where(po => po.PropertyId == propertyId)
                .Join(_context.Owners,
                      po => po.OwnerId,
                      o => o.OwnerId,
                      (po, o) => new { o.FirstName, o.LastName })
                .FirstOrDefaultAsync();

            return owner != null ? $"{owner.FirstName} {owner.LastName}" : "Unknown Owner";
        }
        public async Task<int> GetInvoiceTypeIdByNameAsync(string invoiceTypeName)
        {
            try
            {
                return await _context.LkupInvoiceType
                    .AsNoTracking()
                    .Where(t => t.InvoiceType == invoiceTypeName)
                    .Select(t => t.InvoiceTypeId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve InvoiceTypeName for ID {InvoiceTypeId}", invoiceTypeName);
                return -1;
            }
        }
        public async Task<string> GetInvoiceTypeNameByIdAsync(int invoiceTypeid)
        {
            try
            {
                var invoiceTypeName = await _context.LkupInvoiceType
                     .AsNoTracking()
                     .Where(t => t.InvoiceTypeId == invoiceTypeid)
                     .Select(t => t.InvoiceType)
                     .FirstOrDefaultAsync();

                return invoiceTypeName ?? "unknown";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve InvoiceTypeName for ID {InvoiceTypeId}", invoiceTypeid);
                return "unknown";
            }
        }
        public async Task<int> InvoiceTypeExistsAsync(string invoiceType)
        {
            try
            {
                var invoiceTypeExists = await _context.LkupInvoiceType
                    .AnyAsync(it => it.InvoiceType == invoiceType);
                if (!invoiceTypeExists)
                {
                    _logger.LogWarning("Invalid InvoiceTypeId {InvoiceTypeId}", invoiceType);
                    return -1; // Fix for CS8603: Return a default value for non-nullable type.
                }

                return await GetInvoiceTypeIdByNameAsync(invoiceType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if invoice type exists for ID {invoiceType}", invoiceType);
                return -1; // Fix for CS8603: Return a default value for non-nullable type.
            }
        }
        public async Task<int> GetUtilityTypeNameByIdAsync(string utilityType)
        {
            try
            {
                return await _context.LkupUtilities
                    .AsNoTracking()
                    .Where(t => t.UtilityName == utilityType)
                    .Select(t => t.UtilityId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve utilityType for {utilityType}", utilityType);
                return -1; // Fix for CS8603: Return a default value for non-nullable type.
            }
        }
        public async Task<IEnumerable<Invoice>> GetAllInvoicesForPropertyAsync(int propertyId)
        {
            return await _context.Invoices
                .Where(i => i.PropertyId == propertyId)
                .OrderBy(i => i.CreatedDate)
                .ToListAsync();

        }
        public async Task<IEnumerable<Invoice>> GetAllInvoicesForPropertyAsync(int propertyId, string? status = null)
        {
            var query = _context.Invoices.AsQueryable()
                .Where(i => i.PropertyId == propertyId);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(i => i.Status == status);

            return await query
                .OrderBy(i => i.CreatedDate)
                .ToListAsync();
        }
        public async Task<IEnumerable<Invoice>> GetInvoicesByInvoiceIdAsync(int invoiceId, string? status = null)
        {
            var query = _context.Invoices.AsQueryable()
                .Where(i => i.InvoiceId == invoiceId);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(i => i.Status == status);

            return await query
                .OrderBy(i => i.CreatedDate)
                .ToListAsync();
        }
        public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
        {
            return await _context.Invoices
                .OrderBy(i => i.PropertyId)
                .ThenBy(i => i.CreatedDate)
                .ToListAsync();
        }
        public async Task<decimal> GetTotalAmountByPropertyAsync(int propertyId, string? status = null)
        {
            var query = _context.Invoices
                .Where(i => i.PropertyId == propertyId);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(i => i.Status == status);

            return await query.SumAsync(i => i.Amount);
        }
        public async Task<Dictionary<string, decimal>> GetAmountByTypeAsync(int propertyId)
        {
            return await _context.Invoices
                .Where(i => i.PropertyId == propertyId)
                .GroupBy(i => i.GetType().Name)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(i => i.Amount));
        }
        public async Task<Dictionary<string, decimal>> GetMonthlyTotalsAsync(int propertyId, int year)
        {
            return await _context.Invoices
                .Where(i => i.PropertyId == propertyId && i.CreatedDate.Year == year)
                .GroupBy(i => i.CreatedDate.Month.ToString("D2"))
                .ToDictionaryAsync(g => g.Key, g => g.Sum(i => i.Amount));
        }
        public async Task<IEnumerable<Invoice>> GetFilteredAsync(int propertyId, string? type, string? status, DateTime? dueBefore)
        {
            var query = _context.Invoices
                .Where(i => i.PropertyId == propertyId);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(i => i.Status == status);

            if (dueBefore.HasValue)
                query = query.Where(i => i.DueDate <= dueBefore.Value);

            var invoices = await query.ToListAsync(); // Execute SQL first

            if (!string.IsNullOrWhiteSpace(type))
                invoices = invoices.Where(i => i.GetType().Name == type).ToList(); // Filter in memory

            return invoices;
        }
        public async Task<decimal> GetBalanceForwardAsync(int propertyId, DateTime asOfDate)
        {
            return await _context.Invoices
                .Where(i => i.PropertyId == propertyId && i.DueDate < asOfDate && !i.IsPaid)
                .SumAsync(i => i.Amount);
        }
        public async Task<Invoice?> GetInvoiceByIdAsync(int invoiceId)
        {
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            return invoice;
        }
        public async Task<List<CumulativeInvoiceDto>> ExportExcel(int propertyId)
        {
            var invoices = await GetAllInvoicesForPropertyAsync(propertyId);

            var dto = invoices.Select(async i => new CumulativeInvoiceDto
            {
                InvoiceId = i.InvoiceId,
                PropertyId = i.PropertyId,
                Amount = i.Amount,
                CreatedDate = i.CreatedDate,
                Notes = i.Notes,
                InvoiceType = await GetInvoiceTypeNameByIdAsync(i.InvoiceTypeId)
            });

            return (List<CumulativeInvoiceDto>)dto;


        }
        public async Task<List<CumulativeInvoiceDto>> ExportInvoicesByPropertyIdAsync(int propertyId)
        {
            var invoices = await GetAllInvoicesForPropertyAsync(propertyId);
            var dto = invoices.Select(i => new CumulativeInvoiceDto
            {
                InvoiceId = i.InvoiceId,
                PropertyId = i.PropertyId,
                Amount = i.Amount,
                CreatedDate = i.CreatedDate,
                Notes = i.Notes,
                InvoiceType = i.GetType().Name
            });

            return dto.ToList();
        }
        public async Task<List<CumulativeInvoiceDto>> ExportInvoicesByInvoiceIdAsync(int invoiceId)
        {
            var invoices = await GetInvoicesByInvoiceIdAsync(invoiceId);
            var dto = invoices.Select(async i => new CumulativeInvoiceDto
            {
                InvoiceId = i.InvoiceId,
                PropertyId = i.PropertyId,
                Amount = i.Amount,
                CreatedDate = i.CreatedDate,
                Notes = i.Notes,
                InvoiceType = await GetInvoiceTypeNameByIdAsync(i.InvoiceTypeId)
            });

            return (List<CumulativeInvoiceDto>)dto;
        }
        public async Task<List<CumulativeInvoiceDto>> ExportAllInvoicesAsync()
        {
            var invoices = await GetAllInvoicesAsync();
            var dto = invoices.Select(i => new CumulativeInvoiceDto
            {
                InvoiceId = i.InvoiceId,
                PropertyId = i.PropertyId,
                Amount = i.Amount,
                CreatedDate = i.CreatedDate,
                Notes = i.Notes,
                InvoiceType = i.GetType().Name
            });

            return dto.ToList();
        }
        public async Task<List<CumulativeInvoiceDto>> SendCumulativeInvoiceAsync(int propertyId, string recipientEmail)
        {
            var invoices = await GetAllInvoicesForPropertyAsync(propertyId);

            if (!invoices.Any())
            {
                return new List<CumulativeInvoiceDto>(); // ✅ Return an empty list instead of invalid cast
            }

            var dto = new List<CumulativeInvoiceDto>();

            foreach (var i in invoices)
            {
                dto.Add(new CumulativeInvoiceDto
                {
                    InvoiceId = i.InvoiceId,
                    PropertyId = i.PropertyId,
                    CustomerName = i.CustomerName,
                    Amount = i.Amount,
                    CreatedDate = i.CreatedDate,
                    DueDate = i.DueDate,
                    Notes = i.Notes,
                    Status = i.Status,
                    InvoiceType = await GetInvoiceTypeNameByIdAsync(i.InvoiceTypeId) // ✅ Use await instead of .Result
                });
            }

            var pdfBytes = await _exportService.ExportToPdfAsync(dto);
            var pdfPath = Path.Combine(Path.GetTempPath(), $"cumulative_invoice_{propertyId}.pdf");
            await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);

            // ✅ Send email with the generated PDF attachment
            await _emailService.SendInvoiceEmailAsync(recipientEmail,  pdfPath);

            return dto; // ✅ Return the properly converted list
        }

        public async Task<List<CumulativeInvoiceDto>> SendInvoiceAsync(int invoiceId, string recipientEmail)
        {
            var invoices = await GetInvoicesByInvoiceIdAsync(invoiceId);

            if (!invoices.Any())
            {
                return new List<CumulativeInvoiceDto>(); // ✅ Return an empty list instead of invalid cast
            }

            var dto = new List<CumulativeInvoiceDto>();

            foreach (var i in invoices)
            {
                dto.Add(new CumulativeInvoiceDto
                {
                    InvoiceId = i.InvoiceId,
                    PropertyId = i.PropertyId,
                    CustomerName = i.CustomerName,
                    Amount = i.Amount,
                    CreatedDate = i.CreatedDate,
                    DueDate = i.DueDate,
                    Notes = i.Notes,
                    Status = i.Status,
                    InvoiceType = await GetInvoiceTypeNameByIdAsync(i.InvoiceTypeId) // ✅ Use await instead of .Result
                });
            }

            var pdfBytes = await _exportService.ExportToPdfAsync(dto);
            var pdfPath = Path.Combine(Path.GetTempPath(), $"cumulative_invoice_{invoiceId}.pdf");
            await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);

            // ✅ Send email with the generated PDF attachment
            await _emailService.SendInvoiceEmailAsync(recipientEmail, pdfPath);

            return dto; // ✅ Return the properly converted list
        }
        public async Task<List<CumulativeInvoiceDto>> GetByPropertyAsync(int propertyId, [FromQuery] string? type = null, [FromQuery] string? status = null, [FromQuery] DateTime? dueBefore = null)
        {
            var invoices = await GetFilteredAsync(propertyId, type, status, dueBefore);

            var result = invoices.Select(i => new CumulativeInvoiceDto
            {
                InvoiceId = i.InvoiceId,
                PropertyId = i.PropertyId,
                Amount = i.Amount,
                CreatedDate = i.CreatedDate,
                DueDate = i.DueDate,
                Status = i.Status,
                Notes = i.Notes,
                InvoiceType = i.GetType().Name
            });

            if (result == null || !result.Any())
            {
                _logger.LogWarning("No invoices found for PropertyId {PropertyId}", propertyId);
                return new List<CumulativeInvoiceDto>();
            }

            return result.ToList();
        }
        public async Task<SummaryDto> GetSummaryAsync(int propertyId)
        {
            var invoice = await _context.Invoices
                .Where(i => i.PropertyId == propertyId) // ✅ Filter by PropertyId
                .Join(_context.LkupInvoiceType,
                        i => i.InvoiceTypeId,
                        l => l.InvoiceTypeId,
                        (i, l) => new InvoiceDto
                        {
                            InvoiceId = i.InvoiceId,
                            PropertyId = i.PropertyId,
                            Amount = i.Amount,
                            CreatedDate = i.CreatedDate,
                            DueDate = i.DueDate,
                            Status = i.Status,
                            Notes = i.Notes,
                            InvoiceType = l.InvoiceType
                        })
                .OrderBy(i => i.CreatedDate) // ✅ Sort by CreatedDate
                .ToListAsync();


            var grouped = invoice.GroupBy(d => d.InvoiceType)
                             .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

            var monthly = invoice.GroupBy(d => d.CreatedDate.ToString("yyyy-MM"))
                             .ToDictionary(g => g.Key, g => g.Sum(x => x.Amount));

            return new SummaryDto
            {
                TotalAmount = invoice.Sum(x => x.Amount), // ✅ Use invoices instead of dto
                Count = invoice.Count, // ✅ Use invoices instead of dto
                BreakdownByType = grouped,
                MonthlyTotals = monthly
            };

        }
    }
}
