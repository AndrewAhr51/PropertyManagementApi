﻿using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs.Invoice;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Common.Helpers;

namespace PropertyManagementAPI.Infrastructure.Repositories.Invoices
{
    public class UtilityInvoiceRepository : IUtilityInvoiceRepository
    {
        private readonly MySqlDbContext _context;
        private readonly ILogger<RentInvoiceRepository> _logger;
        private readonly IInvoiceRepository _invoiceRepository;

        public UtilityInvoiceRepository(MySqlDbContext context, ILogger<RentInvoiceRepository> logger, IInvoiceRepository invoiceRepository)
        {
            _context = context;
            _logger = logger;
            _invoiceRepository = invoiceRepository;
        }

        public async Task<bool> CreateUtilitiesInvoiceAsync(UtilityInvoiceCreateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("InvoiceRent is null");
                    return false;
                }

                var invoiceTypeId = await _invoiceRepository.InvoiceTypeExistsAsync(dto.InvoiceType);
                if (invoiceTypeId == null)
                {
                    throw new ArgumentException($"Invalid invoice type: {dto.InvoiceType}");
                }

                  var customerInvoiceInfo = await _invoiceRepository.GetPropertyTenantInfoAsync(dto.PropertyId);
                if (customerInvoiceInfo == null)
                {
                    _logger.LogWarning("No tenant information found for PropertyId {PropertyId}", dto.PropertyId);
                    return false;
                }

                var prevAmountDueTask = _invoiceRepository.GetAmountDueAsync(dto, dto.UtilityType);
                decimal prevAmountDue = await prevAmountDueTask;

                if (prevAmountDue > 0)
                {
                    dto.Amount += prevAmountDue ;
                    _logger.LogWarning("No The utilities were not paid the previous month for PropertyId {PropertyId}", dto.PropertyId);
                }
                else {
                    _logger.LogInformation("Amount due for TenantId {TenantId} is {AmountDue}", dto.PropertyId, dto.Amount);
                }
                var referenceNumber = ReferenceNumberHelper.Generate("REF", dto.PropertyId);

                var newinvoice = new UtilityInvoice
                {
                    PropertyId = dto.PropertyId,
                    ReferenceNumber = referenceNumber,
                    TenantId = customerInvoiceInfo.TenantId,
                    CustomerName = customerInvoiceInfo.CustomerName,
                    Email = customerInvoiceInfo.Email,
                    Amount = dto.Amount,
                    DueDate = dto.DueDate,
                    InvoiceTypeId = invoiceTypeId,
                    Notes = dto.Notes,
                    CreatedDate = DateTime.UtcNow
                };

                _context.UtilityInvoices.Add(newinvoice);
                var saved = await _context.SaveChangesAsync() > 0;

                return saved;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating utility invoice for PropertyId {PropertyId}", dto.PropertyId);
                return false;
            }
        }

        public async Task<UtilityInvoice?> GetUtilitiesInvoiceByIdAsync(int invoiceId)
        {
            return await _context.UtilityInvoices.FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);
        }

        public async Task<IEnumerable<UtilityInvoice>> GetAllUtilitiesInvoiceAsync()
        {
            return await _context.UtilityInvoices.ToListAsync();
        }

        public async Task UpdateUtilitiesInvoiceAsync(UtilityInvoice invoice)
        {
            _context.UtilityInvoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteUtilitiesInvoiceAsync(int invoiceId)
        {
            try
            {
                _context.Invoices.Remove(new Invoice { InvoiceId = invoiceId });
                var save = await _context.SaveChangesAsync();
                return save > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Utilities Invoice invoice with InvoiceId {InvoiceId}", invoiceId);
                return false;
            }
            
        }

        public async Task<int> UtilityTypeExistsAsync(string utilityType)
        {
            try
            {
                var utilityId = await _context.LkupUtilities
                    .AsNoTracking()
                    .Where(u => u.UtilityName == utilityType)
                    .Select(u => u.UtilityId)
                    .FirstOrDefaultAsync();

                if (utilityId == 0)
                {
                    _logger.LogWarning("Utility type '{UtilityType}' not found in LkupUtilities.", utilityType);
                    return -1;
                }

                return utilityId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while retrieving UtilityTypeId for '{UtilityType}'", utilityType);
                return -1;
            }
        }
    }
}