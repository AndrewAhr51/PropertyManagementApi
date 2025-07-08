using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using PropertyManagementAPI.Domain.DTOs.Users;
using PropertyManagementAPI.Domain.Entities.User;
using PropertyManagementAPI.Infrastructure.Repositories.Tenants;

namespace PropertyManagementAPI.Application.Services.Tenants
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _repository;

        public TenantService(ITenantRepository repository)
        {
            _repository = repository;
        }

        public async Task<TenantDto> CreateTenantsAsync(TenantDto dto)
        {
            return await _repository.AddTenantAsync(dto);
        }

        public async Task<IEnumerable<TenantDto>> GetAllTenantsAsync()
        {
            return await _repository.GetAllTenantsAsync();
        }

        public async Task<TenantDto?> GetTenantByIdAsync(int tenantId)
        {
            return await _repository.GetTenantByIdAsync(tenantId);
        }

        public async Task<List<TenantDto?>> GetTenantByPropertyIdAsync(int tenantId)
        {
            return await _repository.GetTenantByPropertyIdAsync(tenantId);
        }

        public async Task<bool> UpdateTenantAsync(int tenantId, TenantDto dto)
        {
            return await _repository.UpdateTenantAsync(dto);
        }

        public async Task<bool> SetActivateTenant(int tenantId)
        {
            return await _repository.SetActivateTenant(tenantId);
        }

        public async Task<bool> LinkQuickBooksAccountAsync(int tenantId, string AccessToken, string RefreshToken, string realmId)
        {
            return await _repository.LinkQuickBooksAccountAsync(tenantId, AccessToken, RefreshToken, realmId);
        }
    }
}