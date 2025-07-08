using DocumentFormat.OpenXml.Office2010.Excel;
using Intuit.Ipp.Data;
using Intuit.Ipp.WebhooksService;
using PropertyManagementAPI.Domain.DTOs.Invoices;
using PropertyManagementAPI.Domain.DTOs.Invoices.Mappers;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;
using Stripe;
using Invoice = PropertyManagementAPI.Domain.Entities.Invoices.Invoice;
using InvoiceLineItem = PropertyManagementAPI.Domain.Entities.Invoices.InvoiceLineItem;

namespace PropertyManagementAPI.Application.Services.Invoices
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _inventoryRepository;

        public InvoiceService(IInvoiceRepository repo) => _inventoryRepository = repo;

        public async Task<InvoiceDto> GetInvoiceByIdAsync(int id)
        {
            var invoice = await _inventoryRepository.GetInvoiceByIdAsync(id);
            if (invoice == null)
                return null;

            var lineItems = await _inventoryRepository.GetLineItemsForInvoiceAsync(id);

            var dto = new InvoiceDto
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
                LineItems = lineItems ?? new List<InvoiceLineItemDto>()
            };

            return dto;
        }

        public async Task<List<InvoiceDto>> GetAllInvoicesAsync()
        {
            var invoices = await _inventoryRepository.GetAllInvoicesAsync();

            return invoices.Select(invoice => new InvoiceDto
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
            }).ToList();
        }

        public async Task<bool> CreateInvoiceAsync(CreateInvoiceDto dto)
        {
            var save = await _inventoryRepository.CreateInvoiceAsync(dto);
            return save;
        }

        public async Task<bool> UpdateInvoiceAsync(InvoiceDto dto)
        {
            var save = await _inventoryRepository.UpdateInvoiceAsync(dto);
            return save;
        }

        public async Task<bool> DeleteInvoiceAsync(int invoiceId)
        {
            var save = await _inventoryRepository.DeleteInvoiceAsync(invoiceId);
            return save;
        }

        public async Task<List<InvoiceLineItemDto>> GetLineItemsForInvoiceAsync(int invoiceId)
        {
            var items = await _inventoryRepository.GetLineItemsForInvoiceAsync(invoiceId);
            return items; 
        }

        // Mapping helpers...
        private async Task<InvoiceDto> MapInvoiceToDto(Domain.Entities.Invoices.Invoice invoice)
        {
            var items = await _inventoryRepository.MapInvoiceToDto(invoice);
            if (items is null)
            {
                return new InvoiceDto();
            }

            return items;
        }
        //private static InvoiceLineItemDto MapLineItemToDto(LineItemType item)
        //{
        //    if (item is null)
        //        throw new ArgumentNullException(nameof(item));

        //    return new InvoiceLineItemDto
        //    {
        //        LineItemId = item.LineItemTypeId,
        //        InvoiceId = item.InvoiceId,
        //        LineItemTypeId = item.LineItemTypeId,
        //        LineItemTypeName = item.InvoiceType?.Name ?? string.Empty,
        //        Description = item.Description ?? string.Empty,
        //        Amount = item.Amount,
        //        Metadata = item.Metadata?.Select(m => new InvoiceLineItemMetadataDto
        //        {
        //            MetaKey = m.MetaKey,
        //            MetaValue = m.MetaValue
        //        }).ToList() ?? new List<InvoiceLineItemMetadataDto>()
        //    };
        //}

        public async Task<int> CreateLineItemAsync(CreateInvoiceLineItemDto dto) =>
            await _inventoryRepository.CreateLineItemAsync(dto);

        public async Task<InvoiceLineItem?> GetLineItemAsync(int lineItemId)
        {
            var entity = await _inventoryRepository.GetLineItemAsync(lineItemId);
            if (entity is null)
                return null;

            return entity;
            
        }

        public async Task<List<InvoiceLineItem>> GetLineItemsByInvoiceIdAsync(int invoiceId)
        {
            var entities = await _inventoryRepository.GetLineItemsByInvoiceIdAsync(invoiceId);
            return entities;
        }

        public async Task<bool> UpdateLineItemAsync(int lineItemId, CreateInvoiceLineItemDto dto) =>
            await _inventoryRepository.UpdateLineItemAsync(lineItemId, dto);

        public async Task<bool> DeleteLineItemAsync(int lineItemId) =>
            await _inventoryRepository.DeleteLineItemAsync(lineItemId);
    }
}
