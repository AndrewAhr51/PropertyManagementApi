using PropertyManagementAPI.Domain.DTOs.Invoices;
using PropertyManagementAPI.Domain.Entities.Invoices;
using Stripe;

namespace PropertyManagementAPI.Domain.DTOs.Invoices.Mappers
{
    public static class InvoiceMapper
    {
        public static InvoiceDto ToDto(Entities.Invoices.Invoice invoice)
        {
            return new InvoiceDto
            {
                InvoiceId = invoice.InvoiceId,
                TenantName = invoice.TenantName,
                Email = invoice.Email,
                Amount = invoice.Amount,
                DueDate = invoice.DueDate,
                IsPaid = invoice.IsPaid,
                Status = invoice.Status,
                LastMonthDue = invoice.LastMonthDue,
                LastMonthPaid = invoice.LastMonthPaid,
                RentMonth = invoice.RentMonth,
                RentYear = invoice.RentYear,
                PropertyId = invoice.PropertyId,
                PropertyName = invoice.PropertyName,
                TenantId = (int)invoice.TenantId,
                OwnerId = invoice.OwnerId,
                Notes = invoice.Notes,
                CreatedBy = invoice.CreatedBy,
                CreatedDate = invoice.CreatedDate,
                ModifiedDate = invoice.ModifiedDate,
                LineItemCollection = invoice.LineItems?
    .Select(InvoiceMapper.ToDto)
    .ToList() ?? new List<InvoiceLineItemDto>(),
                TypeMappings = invoice.TypeMappings?
                    .Select(tm => new InvoiceTypeMappingDto
                    {
                        InvoiceId = tm.InvoiceId,
                        LineItemTypeId = tm.LineItemTypeId
                    }).ToList() ?? new List<InvoiceTypeMappingDto>()
            };
        }

        public static InvoiceLineItemDto ToDto(InvoiceLineItem lineItem)
        {
            return new InvoiceLineItemDto
            {
                LineItemId = lineItem.LineItemId,
                InvoiceId = lineItem.Invoice,
                LineItemTypeId = lineItem.LineItemTypeId,
                Description = lineItem.Description,
                Amount = lineItem.Amount,
                Metadata = lineItem.Metadata?
                    .Select(meta => new InvoiceLineItemMetadataDto
                    {
                        MetaKey = meta.Key,
                        MetaValue = meta.Value
                    }).ToList() ?? new List<InvoiceLineItemMetadataDto>()
            };
        }
    }

}
