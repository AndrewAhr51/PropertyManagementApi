using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;
using PropertyManagementAPI.Infrastructure.Data;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments.CardTokens
{
    public class CardTokenRepository : ICardTokenRepository
    {
        private readonly MySqlDbContext _context;

        public CardTokenRepository(MySqlDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddCardTokenAsync(CardToken token)
        {
            await _context.CardTokens.AddAsync(token);
            await _context.SaveChangesAsync();
            return token.CardTokenId;
        }

        public async Task<CardToken> GetCardTokenByIdAsync(int cardTokenId)
        {
            var cardToken = await _context.CardTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(ct => ct.CardTokenId == cardTokenId);
            if (cardToken == null)
                {
                throw new KeyNotFoundException($"Card token with ID {cardTokenId} not found.");
            }
            return cardToken;
        }

        public async Task<IEnumerable<CardToken>> GetCardTokenByTenantAsync(int tenantId)
        {
            var cardToken = await _context.CardTokens
                .Where(ct => ct.TenantId == tenantId)
                .ToListAsync();
            if (cardToken == null || !cardToken.Any())
                {
                throw new KeyNotFoundException($"No card tokens found for tenant with ID {tenantId}.");
            }
            return cardToken;
        }

        public async Task SetCardTokenDefaultAsync(int cardTokenId, int tenantId)
        {
            var tokens = await _context.CardTokens
                .Where(ct => ct.TenantId == tenantId)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsDefault = token.CardTokenId == cardTokenId;
            }

            await _context.SaveChangesAsync();
        }

        public async Task ClearCardTokenDefaultAsync(int tenantId, int ownerId)
        {
            var tokens = await _context.CardTokens
                .Where(ct => ct.TenantId == tenantId && ct.OwnerId == ownerId && ct.IsDefault)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsDefault = false;
            }

            await _context.SaveChangesAsync();
        }
    }
}
