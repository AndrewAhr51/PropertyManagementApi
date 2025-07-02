using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.DTOs.Property.Pricing;
using PropertyManagementAPI.Domain.Entities.Property;
using PropertyManagementAPI.Infrastructure.Data;

public class PricingRepository : IPricingRepository
{
    private readonly MySqlDbContext _context;

    public PricingRepository(MySqlDbContext context)
    {
        _context = context;
    }

    public async Task<PricingDto> AddPricingAsync(PricingDto dto)
    {
        var propertyExists = await _context.Properties.AnyAsync(p => p.PropertyId == dto.PropertyId);
        if (!propertyExists)
            throw new InvalidOperationException($"Property with ID {dto.PropertyId} does not exist.");

        var entity = new Pricing
        {
            PropertyId = dto.PropertyId,
            EffectiveDate = dto.EffectiveDate,
            RentalAmount = dto.RentalAmount,
            DepositAmount = dto.DepositAmount,
            LeaseTerm = dto.LeaseTerm,
            UtilitiesIncluded = dto.UtilitiesIncluded,
        };

        _context.Pricing.Add(entity);
        await _context.SaveChangesAsync();

        dto.PriceId = entity.PriceId;
        dto.CreatedBy = entity.CreatedBy;
        return dto;
    }

    public async Task<IEnumerable<PricingDto>> GetPricingAllPricingAsync()
    {
        return await _context.Pricing
            .Select(p => new PricingDto
            {
                PriceId = p.PriceId,
                PropertyId = p.PropertyId,
                EffectiveDate = p.EffectiveDate,
                RentalAmount = p.RentalAmount,
                DepositAmount = p.DepositAmount,
                UtilitiesIncluded = p.UtilitiesIncluded,
                LeaseTerm = p.LeaseTerm,

            })
            .ToListAsync();
    }

    public async Task<PricingDto> GetPricingByIdAsync(int id)
    {
        var p = await _context.Pricing
            .FirstOrDefaultAsync(x => x.PriceId == id);

        if (p == null)
            throw new KeyNotFoundException($"Pricing with ID {id} not found.");

        return new PricingDto
        {
            PriceId = p.PriceId,
            PropertyId = p.PropertyId,
            EffectiveDate = p.EffectiveDate,
            RentalAmount = p.RentalAmount,
            DepositAmount = p.DepositAmount,
            LeaseTerm = p.LeaseTerm,
            UtilitiesIncluded = p.UtilitiesIncluded,
            CreatedBy = p.CreatedBy
        };
    }

    public async Task<bool> UpdatePricingAsync(PricingDto dto)
    {
        var entity = await _context.Pricing.FindAsync(dto.PriceId);
        if (entity == null) return false;

        var propertyExists = await _context.Properties.AnyAsync(p => p.PropertyId == dto.PropertyId);
        if (!propertyExists)
            throw new InvalidOperationException($"Property with ID {dto.PropertyId} does not exist.");

        entity.PropertyId = dto.PropertyId;
        entity.EffectiveDate = dto.EffectiveDate;
        entity.RentalAmount = dto.RentalAmount;
        entity.DepositAmount = dto.DepositAmount;
        entity.LeaseTerm = dto.LeaseTerm;
        entity.UtilitiesIncluded = dto.UtilitiesIncluded;

        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<PricingDto?> GetLatestForPropertyAsync(int propertyId)
    {
        var latest = await _context.Pricing
            .Where(p => p.PropertyId == propertyId)
            .OrderByDescending(p => p.EffectiveDate)
            .Select(p => new PricingDto
            {
                PriceId = p.PriceId,
                PropertyId = p.PropertyId,
                EffectiveDate = p.EffectiveDate,
                RentalAmount = p.RentalAmount,
                DepositAmount = p.DepositAmount,
                UtilitiesIncluded = p.UtilitiesIncluded,
                LeaseTerm = p.LeaseTerm,
                CreatedBy = p.CreatedBy
            })
            .FirstOrDefaultAsync();

        return latest;
    }
}