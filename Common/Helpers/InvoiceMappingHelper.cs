using PropertyManagementAPI.Domain.DTOs.Invoices;
using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Common.Helpers
{
    public static class InvoiceMapper
    {
        public static OpenInvoiceByTenantDto OpenInvoiceByTenantList(int tenantId, List<Invoice> invoices)
        {
            return new OpenInvoiceByTenantDto
            {
                TenantId = tenantId,
                TenantName = invoices.FirstOrDefault()?.TenantName ?? "Unknown",
                TotalInvoices = invoices.Count,

                Invoices = invoices.Select(invoice => new InvoiceDto
                {
                    InvoiceId = invoice.InvoiceId,
                    TenantName = invoice.TenantName,
                    Email = invoice.Email,
                    Amount = invoice.Amount,
                    LastMonthDue = invoice.LastMonthDue,
                    LastMonthPaid = invoice.LastMonthPaid,
                    RentMonth = invoice.RentMonth,
                    RentYear = invoice.RentYear,
                    PropertyId = invoice.PropertyId,
                    PropertyName = invoice.PropertyName,
                    TenantId = invoice.TenantId,
                    OwnerId = invoice.OwnerId,
                    DueDate = invoice.DueDate,
                    IsPaid = invoice.IsPaid,
                    Status = invoice.Status,
                    Notes = invoice.Notes,
                    CreatedBy = invoice.CreatedBy,
                    CreatedDate = invoice.CreatedDate,
                    ModifiedDate = invoice.ModifiedDate,

                    LineItems = invoice.LineItems?.Select(li => new InvoiceLineItemDto
                    {
                        LineItemId = li.LineItemId,
                        InvoiceId = li.InvoiceId,
                        LineItemTypeId = li.LineItemTypeId,
                        Description = li.Description,
                        Amount = li.Amount,
                        Metadata = (li.Metadata ?? new List<InvoiceLineItemMetadata>())
                            .Select(m => new InvoiceLineItemMetadataDto
                            {
                                MetaKey = m.MetaKey,
                                MetaValue = m.MetaValue
                            }).ToList()
                    }).ToList() ?? new List<InvoiceLineItemDto>()
                }).ToList()
            };
        }

    }
}
