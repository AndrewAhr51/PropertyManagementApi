using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PropertyManagementAPI.Application.Services.Email;
using PropertyManagementAPI.Application.Services.InvoiceExport;
using PropertyManagementAPI.Common.Helpers;
using PropertyManagementAPI.Domain.DTOs;
using PropertyManagementAPI.Domain.DTOs.Invoices;
using PropertyManagementAPI.Domain.DTOs.Invoices.Mappers;
using PropertyManagementAPI.Domain.DTOs.Property;
using PropertyManagementAPI.Domain.DTOs.Users;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Domain.Entities.Property;
using PropertyManagementAPI.Infrastructure.Data;
using Stripe = Stripe.Invoice;
using System.Linq;
using Invoice = PropertyManagementAPI.Domain.Entities.Invoices.Invoice;
using PropertyManagementAPI.Domain.Entities.User;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly MySqlDbContext _context;
        private readonly ILogger<InvoiceDto> _logger;
        private readonly IExportService<InvoiceDto> _exportService;
        private readonly IEmailService _emailService;

        public InvoiceRepository(MySqlDbContext context, ILogger<InvoiceDto> logger, IExportService<InvoiceDto> exportService, IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _exportService = exportService;
            _emailService = emailService;
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int invoiceId)
        {
            try
            {
                _logger.LogInformation("🔎 Attempting to fetch invoice with ID {InvoiceId}", invoiceId);

                var invoice = await _context.Set<Invoice>()
                    .Include(i => i.LineItems)
                    .ThenInclude(li => li.InvoiceType) // ✅ Add this to load InvoiceType
                    .Include(i => i.LineItems)
                    .ThenInclude(li => li.Metadata)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

                if (invoice is null)
                {
                    _logger.LogWarning("⚠️ Invoice with ID {InvoiceId} not found", invoiceId);
                    return null;
                }

                _logger.LogInformation("✅ Invoice {InvoiceId} retrieved. LineItems: {LineItemCount}",
                    invoiceId, invoice.LineItems?.Count ?? 0);

                return invoice;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ An error occurred while retrieving invoice ID {InvoiceId}", invoiceId);
                return null;
            }
        }

        public async Task<InvoiceDto?> GetInvoiceAsync(int invoiceId)
        {
            try
            {
                _logger.LogInformation("📄 Retrieving Invoice with ID {InvoiceId}", invoiceId);

                var entity = await GetInvoiceByIdAsync(invoiceId);
                if (entity is null)
                {
                    _logger.LogWarning("❌ Invoice with ID {InvoiceId} not found", invoiceId);
                    return null;
                }

                var lineItems = await GetLineItemsForInvoiceAsync(invoiceId);
                _logger.LogInformation("🧾 Retrieved {Count} line items for Invoice ID {InvoiceId}", lineItems?.Count ?? 0, invoiceId);

                var dto = new InvoiceDto
                {
                    InvoiceId = entity.InvoiceId,
                    TenantName = entity.TenantName,
                    Email = entity.Email,
                    Amount = entity.Amount,
                    LastMonthDue = entity.LastMonthDue,
                    LastMonthPaid = entity.LastMonthPaid,
                    RentMonth = entity.RentMonth,
                    RentYear = entity.RentYear,
                    PropertyId = entity.PropertyId,
                    PropertyName = entity.PropertyName,
                    TenantId = entity.TenantId,
                    OwnerId = entity.OwnerId,
                    DueDate = entity.DueDate,
                    IsPaid = entity.IsPaid,
                    Status = entity.Status,
                    Notes = entity.Notes,
                    CreatedBy = entity.CreatedBy,
                    CreatedDate = entity.CreatedDate,
                    ModifiedDate = entity.ModifiedDate,
                    LineItems = lineItems ?? new List<InvoiceLineItemDto>()
                };

                _logger.LogInformation("✅ Invoice DTO constructed successfully for ID {InvoiceId}", invoiceId);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔥 Error occurred while mapping Invoice DTO for ID {InvoiceId}", invoiceId);
                return null;
            }
        }

        public async Task<List<Invoice>> GetAllInvoicesAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving all invoices from database...");

                var invoices = await _context.Set<Invoice>().ToListAsync();

                _logger.LogInformation("Retrieved {InvoiceCount} invoice(s).", invoices.Count);

                return invoices;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all invoices.");
                return new List<Invoice>();
            }
        }

        public async Task<IEnumerable<OpenInvoiceByTenantDto>> GetAllInvoicesByTenantIdAsync(int tenantId)
        {
            try
            {
                _logger.LogInformation("Retrieving all invoices for the {tenantId} from database...", tenantId);

                var invoices = await _context.Invoices
                    .Include(i => i.LineItems)
                        .ThenInclude(li => li.Metadata)
                    .Where(i => i.TenantId == tenantId)
                    .ToListAsync();

                var invoiceByTenantList = InvoiceMapper.OpenInvoiceByTenantList(tenantId, invoices);

                _logger.LogInformation("Retrieved {InvoiceCount} invoice(s).", invoices.Count);

                // Fix: Wrap single object in a collection for correct return type
                return new List<OpenInvoiceByTenantDto> { invoiceByTenantList };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all invoices.");
                // Fix: Return an empty collection for correct return type
                return Enumerable.Empty<OpenInvoiceByTenantDto>();
            }
        }

        public async Task<bool> CreateInvoiceAsync(CreateInvoiceDto invoice)
        {
            if (invoice == null)
            {
                _logger.LogWarning("Invoice DTO is null");
                return false;
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Creating invoice for PropertyId: {PropertyId}", invoice.PropertyId);

                var tenant = await GetPropertyTenantInfoAsync(invoice.PropertyId);
                if (tenant == null)
                {
                    _logger.LogWarning("Tenant not found for PropertyId: {PropertyId}", invoice.PropertyId);
                    return false;
                }

                var invoiceInfo = await GetInvoiceInfoAsync(invoice);
                _logger.LogInformation("Retrieved previous invoice info for TenantId: {TenantId}", tenant.TenantId);

                var referenceNumber = ReferenceNumberHelper.Generate("REF", invoice.PropertyId);

                var newInvoice = new Invoice
                {
                    PropertyName = tenant.PropertyName,
                    OwnerId = tenant.OwnerId,
                    TenantId = tenant.TenantId,
                    TenantName = tenant.TenantName,
                    Email = tenant.TenantEmail,
                    ReferenceNumber = referenceNumber,
                    Amount = invoiceInfo.Amount,
                    DueDate = invoice.DueDate,
                    LastMonthDue = invoiceInfo.LastMonthDue,
                    LastMonthPaid = invoiceInfo.LastMonthPaid,
                    PropertyId = invoice.PropertyId,
                    RentMonth = invoice.DueDate.Month,
                    RentYear = invoice.DueDate.Year,
                    Notes = invoice.Notes,
                    CreatedDate = DateTime.UtcNow,
                    IsPaid = false // Explicit if you support this flag in schema
                };

                _context.Invoices.Add(newInvoice);

                // ⬆️ Optional: Directly adjust tenant balance upfront if needed
                var tenantRecord = await _context.Tenants.FindAsync(tenant.TenantId);
                if (tenantRecord != null)
                {
                    tenantRecord.Balance += newInvoice.Amount;
                    _context.Tenants.Update(tenantRecord);
                }

                var save = await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Invoice created successfully with InvoiceId: {InvoiceId}", newInvoice.InvoiceId);
                return save > 0;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error occurred while creating invoice for PropertyId: {PropertyId}", invoice?.PropertyId);
                return false;
            }
        }

        public async Task<bool> UpdateInvoiceAsync(InvoiceDto invoiceDto)
        {
            if (invoiceDto == null)
            {
                _logger.LogWarning("Invoice update failed — provided InvoiceDto is null.");
                return false;
            }

            _logger.LogInformation("Beginning atomic update for InvoiceId: {InvoiceId}", invoiceDto.InvoiceId);

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var entity = await _context.Invoices.FindAsync(invoiceDto.InvoiceId);
                if (entity == null)
                {
                    _logger.LogWarning("InvoiceId {InvoiceId} not found in database.", invoiceDto.InvoiceId);
                    return false;
                }

                // ✅ Update invoice fields
                entity.TenantName = invoiceDto.TenantName ?? string.Empty;
                entity.Email = invoiceDto.Email ?? string.Empty;
                entity.Amount = invoiceDto.Amount;
                entity.DueDate = invoiceDto.DueDate;
                entity.IsPaid = invoiceDto.IsPaid;
                entity.Status = invoiceDto.Status ?? string.Empty;
                entity.Notes = invoiceDto.Notes ?? string.Empty;
                entity.ModifiedDate = DateTime.UtcNow;

                // ✅ Update linked line items if provided
                if (invoiceDto.LineItems?.Any() == true)
                {
                    var existingLineItems = await _context.InvoiceLineItems
                        .Where(li => li.InvoiceId == invoiceDto.InvoiceId)
                        .Include(li => li.Metadata)
                        .ToListAsync();

                    _context.InvoiceLineItems.RemoveRange(existingLineItems);

                    var mappedLineItems = invoiceDto.LineItems.Select(dto => new InvoiceLineItem
                    {
                        InvoiceId = invoiceDto.InvoiceId,
                        LineItemTypeId = dto.LineItemTypeId ?? throw new InvalidOperationException("LineItemTypeId must not be null."),
                        Description = dto.Description ?? string.Empty,
                        Amount = dto.Amount,
                        Metadata = dto.Metadata?.Select(m => new InvoiceLineItemMetadata
                        {
                            MetaKey = m.MetaKey ?? string.Empty,
                            MetaValue = m.MetaValue ?? string.Empty,
                            CreatedDate = DateTime.UtcNow
                        }).ToList() ?? new List<InvoiceLineItemMetadata>()
                    }).ToList();

                    _context.InvoiceLineItems.AddRange(mappedLineItems);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("InvoiceId {InvoiceId} and its line items updated successfully.", invoiceDto.InvoiceId);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Atomic update failed for InvoiceId: {InvoiceId}", invoiceDto.InvoiceId);
                return false;
            }
        }

        public async Task<bool> DeleteInvoiceAsync(int invoiceId)
        {
            try
            {
                var invoice = await GetInvoiceByIdAsync(invoiceId);
                if (invoice is null)
                {
                    _logger.LogWarning("Invoice not found for deletion. InvoiceId: {InvoiceId}", invoiceId);
                    return false;
                }

                _context.Invoices.Remove(invoice);

                var save = await _context.SaveChangesAsync();

                _logger.LogInformation("Invoice deleted successfully. InvoiceId: {InvoiceId}", invoiceId);

                return save > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting invoice. InvoiceId: {InvoiceId}", invoiceId);
                return false;
            }
        }

        public async Task<List<InvoiceLineItemDto>> GetLineItemsForInvoiceAsync(int invoiceId)
        {
            try
            {
                var lineItems = await _context.Set<InvoiceLineItem>()
                    .Where(li => li.InvoiceId == invoiceId)
                    .Include(li => li.Metadata)
                    .Include(li => li.InvoiceType)
                    .Select(li => new InvoiceLineItemDto
                    {
                        LineItemId = li.LineItemId,
                        InvoiceId = li.InvoiceId,
                        LineItemTypeId = li.LineItemTypeId,
                        LineItemTypeName = li.InvoiceType.LineItemTypeName ?? string.Empty,
                        Description = li.Description,
                        Amount = li.Amount,
                        Metadata = li.Metadata.Select(m => new InvoiceLineItemMetadataDto
                        {
                            MetaKey = m.MetaKey ?? string.Empty,
                            MetaValue = m.MetaValue ?? string.Empty
                        }).ToList()
                    })
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} line items for InvoiceId: {InvoiceId}",
                                       lineItems.Count, invoiceId);

                return lineItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving line items for InvoiceId: {InvoiceId}", invoiceId);
                return new List<InvoiceLineItemDto>();
            }
        }

        public async Task<List<InvoiceLineItemMetadata>> GetMetadataByLineItemIdAsync(int lineItemId)
        {
            try
            {
                var metadata = await _context.Set<InvoiceLineItemMetadata>()
                    .Where(m => m.LineItemId == lineItemId)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} metadata entries for LineItemId: {LineItemId}",
                                       metadata.Count, lineItemId);

                return metadata;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving metadata for LineItemId: {LineItemId}", lineItemId);
                return new List<InvoiceLineItemMetadata>();
            }
        }

        public async Task<InvoiceLineItem?> GetLineItemAsync(int lineItemId)
        {
            try
            {
                var lineItem = await _context.InvoiceLineItems
                    .Include(li => li.Metadata)
                    .Include(li => li.InvoiceType)
                    .FirstOrDefaultAsync(li => li.LineItemId == lineItemId);

                if (lineItem == null)
                {
                    _logger.LogWarning("LineItemId not found: {LineItemId}", lineItemId);
                    return null;
                }

                _logger.LogInformation("Retrieved LineItemId: {LineItemId} with {MetadataCount} metadata entries",
                                       lineItem.LineItemId, lineItem.Metadata?.Count ?? 0);

                return lineItem;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving LineItemId: {LineItemId}", lineItemId);
                return null;
            }
        }

        public async Task<List<InvoiceLineItem>> GetLineItemsByInvoiceIdAsync(int invoiceId)
        {
            try
            {
                var lineItems = await _context.InvoiceLineItems
                    .Where(li => li.InvoiceId == invoiceId)
                    .Include(li => li.Metadata)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} line items for InvoiceId: {InvoiceId}", lineItems.Count, invoiceId);

                return lineItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving line items for InvoiceId: {InvoiceId}", invoiceId);
                return new List<InvoiceLineItem>();
            }
        }

        public async Task<int> CreateLineItemAsync(CreateInvoiceLineItemDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            try
            {
                var invoiceTypeId = await GetLineItemTypeIdAsync(dto.LineItemTypeName);

                var entity = new InvoiceLineItem
                {
                    InvoiceId = dto.InvoiceId,
                    LineItemTypeId = invoiceTypeId,
                    Description = dto.Description ?? string.Empty,
                    Amount = dto.Amount,
                    SortOrder = dto.SortOrder ?? 0,
                    IsDeleted = false,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    Metadata = dto.Metadata?.Select(m => new InvoiceLineItemMetadata
                    {
                        MetaKey = m.MetaKey ?? string.Empty,
                        MetaValue = m.MetaValue ?? string.Empty,
                        CreatedDate = DateTime.UtcNow
                    }).ToList() ?? new List<InvoiceLineItemMetadata>()
                };

                _context.InvoiceLineItems.Add(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created LineItemId: {LineItemId} for InvoiceId: {InvoiceId}",
                                       entity.LineItemId, entity.InvoiceId);

                var invoice = await _context.Invoices
                    .Include(i => i.LineItems)
                    .FirstOrDefaultAsync(i => i.InvoiceId == entity.InvoiceId);

                if (invoice != null)
                {
                    invoice.Amount = invoice.LineItems.Sum(li => li.Amount);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Updated Invoice total for InvoiceId: {InvoiceId}. New total: {Amount:C}",
                                           invoice.InvoiceId, invoice.Amount);
                }

                return entity.LineItemId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating LineItem for InvoiceId: {InvoiceId}", dto.InvoiceId);
                throw; // optional: rethrow or handle gracefully depending on your design
            }
        }

        public async Task<bool> UpdateLineItemAsync(int lineItemId, CreateInvoiceLineItemDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var entity = await _context.InvoiceLineItems
                .Include(li => li.Metadata)
                .FirstOrDefaultAsync(li => li.LineItemId == lineItemId);

            if (entity == null)
                return false;

            // 🔧 Update fields
            entity.LineItemTypeId = await GetLineItemTypeIdAsync(dto.LineItemTypeName);
            entity.Description = dto.Description ?? string.Empty;
            entity.Amount = dto.Amount;

            // 🔁 Replace metadata
            entity.Metadata = dto.Metadata?.Select(m => new InvoiceLineItemMetadata
            {
                MetaKey = m.MetaKey ?? string.Empty,
                MetaValue = m.MetaValue ?? string.Empty,
                CreatedDate = DateTime.UtcNow
            }).ToList() ?? new List<InvoiceLineItemMetadata>();

            // 💰 Recalculate Invoice total
            var invoice = await _context.Invoices
                .Include(i => i.LineItems)
                .FirstOrDefaultAsync(i => i.InvoiceId == entity.InvoiceId);

            if (invoice != null)
            {
                invoice.Amount = invoice.LineItems.Sum(li => li.Amount);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteLineItemAsync(int lineItemId)
        {
            try
            {
                var entity = await _context.InvoiceLineItems.FindAsync(lineItemId);
                if (entity == null)
                {
                    _logger.LogWarning("LineItem not found for deletion. LineItemId: {LineItemId}", lineItemId);
                    return false;
                }

                int invoiceId = entity.InvoiceId;

                _context.InvoiceLineItems.Remove(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted LineItemId: {LineItemId}. Syncing InvoiceId: {InvoiceId} total.", lineItemId, invoiceId);

                var invoice = await _context.Invoices
                    .Include(i => i.LineItems)
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

                if (invoice != null)
                {
                    invoice.Amount = invoice.LineItems.Sum(li => li.Amount);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Updated Invoice total. InvoiceId: {InvoiceId}, New Total: {Total:C}", invoiceId, invoice.Amount);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting LineItemId: {LineItemId}", lineItemId);
                return false;
            }
        }

        public async Task<PropertyInfoDto> GetPropertyTenantInfoAsync(int propertyId)
        {
            try
            {
                var tenant = await _context.Properties
                    .Where(p => p.PropertyId == propertyId)
                    .Join(_context.Tenants,
                        p => p.PropertyId,
                        t => t.PropertyId,
                        (p, t) => new { Property = p, Tenant = t })
                    .Join(_context.PropertyOwners,
                        pt => pt.Property.PropertyId,
                        po => po.PropertyId,
                       (pt, po) => new PropertyInfoDto
                       {
                           TenantId = pt.Tenant.TenantId,
                           TenantName = $"{pt.Tenant.FirstName} {pt.Tenant.LastName}",
                           TenantEmail = pt.Tenant.Email,
                           PropertyName = pt.Property.PropertyName,
                           OwnerId = po.OwnerId
                       })
                 .FirstOrDefaultAsync();

                if (tenant == null)
                {
                    _logger.LogWarning("No tenant found for PropertyId: {PropertyId}", propertyId);
                    return new PropertyInfoDto
                    {
                        TenantId = 0,
                        TenantName = "Unknown Tenant",
                        TenantEmail = "Unknown Email",
                        PropertyName = "Unknown Property",
                        OwnerId = 0,
                    };
                }

                _logger.LogInformation("Tenant found for PropertyId: {PropertyId}: {TenantName}", propertyId, tenant.TenantName);
                return tenant;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tenant info for PropertyId: {PropertyId}", propertyId);
                return new PropertyInfoDto
                {
                    TenantName = "Unknown Tenant",
                    PropertyName = "Unknown Property"
                };
            }
        }

        public async Task<int> GetLineItemTypeIdAsync(string lineItemTypeName)
        {
            var typeId = await _context.Set<lkupLineItemType>()
                .Where(t => t.LineItemTypeName == lineItemTypeName)
                .Select(t => (int?)t.LineItemTypeId)
                .FirstOrDefaultAsync();

            if (typeId == null)
            {
                _logger.LogWarning("Line item type not found for name: {TypeName}", lineItemTypeName);
                throw new InvalidOperationException($"Line item type '{lineItemTypeName}' does not exist.");
            }

            return typeId.Value;
        }

        private async Task<InvoiceInfoDto> GetInvoiceInfoAsync(CreateInvoiceDto invoice)
        {
            try
            {
                var lease = await _context.Leases
                    .AsNoTracking()
                    .Where(l => l.PropertyId == invoice.PropertyId && l.IsActive)
                    .OrderByDescending(l => l.StartDate)
                    .FirstOrDefaultAsync();

                if (lease == null)
                {
                    _logger.LogWarning("No active lease found for PropertyId: {PropertyId}", invoice.PropertyId);
                    return new InvoiceInfoDto
                    {
                        Amount = 0,
                        CurrentBalance = 0,
                        LastMonthDue = 0,
                        LastMonthPaid = 0
                    };
                }

                var discount = lease.Amount * (lease.Discount / 100m);
                var baseRent = lease.Amount - discount;

                var previous = await _context.Invoices
                    .Where(r => r.PropertyId == invoice.PropertyId &&
                                r.Status != "Paid" &&
                                r.DueDate.Month == invoice.DueDate.Month &&
                                r.DueDate.Year == invoice.DueDate.Year)
                    .FirstOrDefaultAsync();

                decimal previousAmount = previous?.Amount ?? 0m;
                decimal lastMonthDue = previous?.LastMonthDue ?? 0m;
                decimal lastMonthPaid = previous?.LastMonthPaid ?? 0m;

                if (previous != null)
                {
                    previousAmount += 50; // Fixed late fee, adjust as needed
                    _logger.LogInformation("Applied late fee to InvoiceId: {InvoiceId}. Total now: {Balance}",
                                           previous.InvoiceId, previousAmount);
                }

                var result = new InvoiceInfoDto
                {
                    Amount = lease.Amount,
                    CurrentBalance = previousAmount,
                    LastMonthDue = lastMonthDue,
                    LastMonthPaid = lastMonthPaid
                };

                _logger.LogInformation("Successfully retrieved invoice info for PropertyId: {PropertyId}", invoice.PropertyId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching invoice info for PropertyId: {PropertyId}", invoice.PropertyId);

                return new InvoiceInfoDto
                {
                    Amount = 0,
                    CurrentBalance = 0,
                    LastMonthDue = 0,
                    LastMonthPaid = 0
                };
            }
        }


        private async Task UpdateInvoiceTotalAsync(int invoiceId)
        {
            try
            {
                var invoice = await _context.Invoices
                    .Include(i => i.LineItems)
                    .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

                if (invoice == null)
                {
                    _logger.LogWarning("Invoice not found for UpdateInvoiceTotalAsync. InvoiceId: {InvoiceId}", invoiceId);
                    return;
                }

                invoice.Amount = invoice.LineItems?.Sum(li => li.Amount) ?? 0m;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated invoice total for InvoiceId: {InvoiceId}. New total: {Amount:C}", invoiceId, invoice.Amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating invoice total for InvoiceId: {InvoiceId}", invoiceId);
            }
        }

        public async Task<InvoiceDto?> MapInvoiceToDto(Invoice invoice)
        {
            try
            {
                var entity = await _context.Invoices
                    .Include(i => i.LineItems)
                    .Where(i => i.InvoiceId == invoice.InvoiceId)
                    .AsNoTracking()
                    .FirstOrDefaultAsync();

                if (entity is null)
                {
                    _logger.LogWarning("No invoice found with InvoiceId: {InvoiceId}", invoice.InvoiceId);
                    return null;
                }

                _logger.LogInformation("Successfully mapped invoice {InvoiceId} to DTO", entity.InvoiceId);

                return new InvoiceDto
                {
                    InvoiceId = entity.InvoiceId,
                    TenantName = entity.TenantName,
                    Email = entity.Email,
                    Amount = entity.Amount,
                    DueDate = entity.DueDate,
                    IsPaid = entity.IsPaid,
                    Status = entity.Status,
                    LineItems = entity.LineItems.Select(li => new InvoiceLineItemDto
                    {
                        LineItemId = li.LineItemId,
                        Description = li.Description,
                        Amount = li.Amount
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to map invoice {InvoiceId} to DTO", invoice.InvoiceId);
                return null;
            }
        }

    }
}
