using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using PayPalCheckoutSdk.Core;
using PropertyManagementAPI.Domain.DTOs.Users;
using PropertyManagementAPI.Domain.Entities.Payments.Quickbooks;
using PropertyManagementAPI.Domain.Entities.User;
using PropertyManagementAPI.Infrastructure.Data;
using PropertyManagementAPI.Infrastructure.Repositories.Users;

namespace PropertyManagementAPI.Infrastructure.Repositories.Tenants
{
    public class TenantRepository : ITenantRepository
    {
        private readonly MySqlDbContext _context;
        private readonly ILogger<TenantRepository> _logger;

        public TenantRepository(MySqlDbContext context, ILogger<TenantRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TenantDto> AddTenantAsync(TenantDto dto)
        {
            _logger.LogInformation("Service Call: Attempting to add tenant {Email}", dto.Email);

            try
            {
                var entity = new Tenant
                {
                    TenantId = dto.UserId,
                    PropertyId = dto.PropertyId,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    PhoneNumber = dto.PhoneNumber,
                    MoveInDate = dto.MoveInDate,
                    Balance = 0.0m // Default balance
                };

                _context.Tenants.Add(entity);
                await _context.SaveChangesAsync();

                dto.TenantId = entity.TenantId;
                dto.CreatedBy = entity.CreatedBy;

                _logger.LogInformation("Service Success: Tenant {TenantId} added", entity.TenantId);
                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service Error: Failed to add tenant {Email}", dto.Email);
                throw; // Optionally return null or a service result object instead
            }
        }

        public async Task<IEnumerable<TenantDto>> GetAllTenantsAsync()
        {
            _logger.LogInformation("Service Call: Starting retrieval of all tenants");

            try
            {
                var tenants = await _context.Tenants
                    .Select(t => new TenantDto
                    {
                        TenantId = t.TenantId,
                        PropertyId = t.PropertyId,
                        FirstName = t.FirstName,
                        LastName = t.LastName,
                        PhoneNumber = t.PhoneNumber,
                        MoveInDate = t.MoveInDate,
                        CreatedBy = t.CreatedBy
                    })
                    .ToListAsync();

                _logger.LogInformation("Service Success: Retrieved {Count} tenants", tenants.Count);

                return tenants;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service Error: Failed to retrieve tenants");
                return Enumerable.Empty<TenantDto>();
            }
        }

        public async Task<TenantDto?> GetTenantByIdAsync(int tenantId)
        {
            _logger.LogInformation("Retrieving tenant with TenantId {TenantId}", tenantId);

            try
            {
                var tenant = await _context.Tenants
                    .Where(t => t.TenantId == tenantId)
                    .Select(t => new TenantDto
                    {
                        TenantId = t.TenantId,
                        PropertyId = t.PropertyId,
                        FirstName = t.FirstName,
                        LastName = t.LastName,
                        Email = t.Email,
                        PhoneNumber = t.PhoneNumber,
                        MoveInDate = t.MoveInDate,
                        Balance = t.Balance,
                        CreatedBy = t.CreatedBy
                    })
                    .FirstOrDefaultAsync();

                if (tenant == null)
                {
                    _logger.LogWarning("No tenant found for TenantId {TenantId}", tenantId);
                }

                return tenant;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tenant with TenantId {TenantId}", tenantId);
                return null;
            }
        }

        public async Task<bool> UpdateTenantAsync(TenantDto dto)
        {
            _logger.LogInformation("Service Call: Attempting to update tenant {TenantId}", dto.TenantId);

            try
            {
                var entity = await _context.Tenants.FindAsync(dto.TenantId);

                if (entity == null)
                {
                    _logger.LogWarning("Update Failed: TenantId {TenantId} not found", dto.TenantId);
                    return false;
                }

                entity.FirstName = dto.FirstName;
                entity.LastName = dto.LastName;
                entity.PhoneNumber = dto.PhoneNumber;
                entity.MoveInDate = dto.MoveInDate;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Service Success: TenantId {TenantId} updated", dto.TenantId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service Error: Failed to update TenantId {TenantId}", dto.TenantId);
                return false;
            }
        }

        public async Task<bool> SetActivateTenant(int tenantId)
        {
            try
            {
                var tenant = await _context.Tenants.FindAsync(tenantId);

                if (tenant == null)
                {
                    _logger.LogWarning("User not found for ID: {Id}", tenantId);
                    return false;
                }

                tenant.IsActive = !tenant.IsActive;

                var save = await _context.SaveChangesAsync();

                var user = await _context.Users.FindAsync(tenantId);
                if (user == null)
                {
                    _logger.LogWarning("User not found for ID: {Id}", tenantId);
                    return false;
                }

                user.IsActive = !user.IsActive;
                save = await _context.SaveChangesAsync();

                return save > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inactivating the user with ID: {Id}", tenantId);
                throw new Exception("An error occurred while deleting the user.", ex);
            }
        }

        public async Task<bool> LinkQuickBooksAccountAsync(int tenantId, string accessToken, string refreshToken, string realmId)
        {
            _logger.LogInformation("LinkQuickBooksAccount: Starting link for TenantId {TenantId}", tenantId);

            try
            {
                var tenant = await GetTenantByIdAsync(tenantId);
                if (tenant is null)
                {
                    _logger.LogWarning("LinkQuickBooksAccount: TenantId {TenantId} not found", tenantId);
                    return false;
                }

                tenant.QuickBooksAccessToken = accessToken;
                tenant.QuickBooksRefreshToken = refreshToken;
                tenant.RealmId = realmId;

                var save = await UpdateTenantAsync(tenant);

                if (save)
                {
                    _logger.LogInformation("LinkQuickBooksAccount: QuickBooks credentials linked for TenantId {TenantId}", tenantId);
                    return true;
                }

                _logger.LogWarning("LinkQuickBooksAccount: Failed to update TenantId {TenantId}", tenantId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LinkQuickBooksAccount: Error linking QuickBooks for TenantId {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<List<TenantDto?>> GetTenantByPropertyIdAsync(int propertyId)
        {
            _logger.LogInformation("Retrieving tenants with Property Id {PropertyId}", propertyId);

            try
            {
                var tenants = await _context.Tenants
                    .Where(t => t.PropertyId == propertyId)
                    .Select(t => new TenantDto
                    {
                        TenantId = t.TenantId,
                        PropertyId = t.PropertyId,
                        FirstName = t.FirstName,
                        LastName = t.LastName,
                        Email = t.Email,
                        PhoneNumber = t.PhoneNumber,
                        MoveInDate = t.MoveInDate,
                        Balance = t.Balance,
                        CreatedBy = t.CreatedBy
                    })
                    .ToListAsync();

                if (tenants == null || tenants.Count == 0)
                {
                    _logger.LogWarning("No tenants found for Property Id {PropertyId}", propertyId);
                    return new List<TenantDto?>(); // ✅ Explicit empty list for clarity
                }

                return tenants;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tenants with Property Id {PropertyId}", propertyId);
                return new List<TenantDto?>(); // ✅ Safe fallback
            }
        }

        public async Task<bool> RecordQuickBooksAuditAsync(int tenantId, string realmId, string eventType, string? correlationId = null)
        {
            try
            {
                var log = new QuickBooksAuditLog
                {
                    TenantId = tenantId,
                    RealmId = realmId,
                    EventType = eventType,
                    CorrelationId = correlationId,
                    TimestampUtc = DateTime.UtcNow
                };

                _context.QuickBooksAuditLogs.Add(log);
               var save = await _context.SaveChangesAsync();
                return save > 0;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update error while recording QuickBooks audit log: tenantId={TenantId}, realmId={RealmId}, eventType={EventType}, correlationId={CorrelationId}",
                    tenantId, realmId, eventType, correlationId);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to record QuickBooks audit log: tenantId={TenantId}, realmId={RealmId}, eventType={EventType}, correlationId={CorrelationId}",
                    tenantId, realmId, eventType, correlationId);
                return false;
            }
        }
    }
}

