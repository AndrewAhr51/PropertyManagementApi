using PropertyManagementAPI.Domain.DTOs.Quickbooks;
using PropertyManagementAPI.Infrastructure.Repositories.Quickbooks;
using System.Collections.Concurrent;

namespace PropertyManagementAPI.Infrastructure.Quickbooks
{
    public class InMemoryTokenStore : ITokenStore
    {
        private readonly ConcurrentDictionary<string, TokenSet> _cache = new();

        public Task<TokenSet?> GetTokenAsync(string realmId)
        {
            _cache.TryGetValue(realmId, out var token);
            return Task.FromResult(token);
        }

        public Task SaveTokenAsync(string realmId, TokenSet token)
        {
            _cache[realmId] = token;
            return Task.CompletedTask;
        }

        public Task DeleteTokenAsync(string realmId)
        {
            _cache.TryRemove(realmId, out _);
            return Task.CompletedTask;
        }
    }
}