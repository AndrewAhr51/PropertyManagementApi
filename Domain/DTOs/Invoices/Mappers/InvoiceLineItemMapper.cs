using PropertyManagementAPI.Domain.Entities.Invoices;

namespace PropertyManagementAPI.Domain.DTOs.Invoices.Mappers
{
    public static class InvoiceLineItemMapper
    {
        public static InvoiceLineItemDto ToDto(InvoiceLineItem entity)
        {
            return new InvoiceLineItemDto
            {
                LineItemId = entity.LineItemId,
                InvoiceId = entity.InvoiceId,
                LineItemTypeId = entity.LineItemTypeId,
                LineItemTypeName = entity.InvoiceType?.LineItemTypeName ?? string.Empty,
                Description = entity.Description,
                Amount = entity.Amount,
                Metadata = entity.Metadata?
                    .Select(m => new InvoiceLineItemMetadataDto
                    {
                        MetaKey = m.MetaKey,
                        MetaValue = m.MetaValue
                    }).ToList() ?? new List<InvoiceLineItemMetadataDto>()
            };
        }

        public static InvoiceLineItem ToEntity(InvoiceLineItemDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.InvoiceId == 0)
                throw new InvalidOperationException("InvoiceId is required.");

            if (!dto.LineItemTypeId.HasValue)
                throw new InvalidOperationException("LineItemTypeId is required.");

            return new InvoiceLineItem
            {
                LineItemId = dto.LineItemId,
                InvoiceId = dto.InvoiceId,
                LineItemTypeId = dto.LineItemTypeId.Value,
                Description = dto.Description ?? string.Empty,
                Amount = dto.Amount,
                Metadata = dto.Metadata?.Select(kvp => new InvoiceLineItemMetadata
                {
                    MetaKey = kvp.MetaKey ?? string.Empty,
                    MetaValue = kvp.MetaValue ?? string.Empty
                }).ToList() ?? new List<InvoiceLineItemMetadata>()
            };
        }

        public static List<InvoiceLineItemDto> ToDtoList(IEnumerable<InvoiceLineItem> items) =>
            items.Select(ToDto).ToList();

        public static List<InvoiceLineItem> ToEntityList(IEnumerable<InvoiceLineItemDto> dtos) =>
            dtos.Select(ToEntity).ToList();
    }
}
