using Intuit.Ipp.Data;
using PropertyManagementAPI.Domain.DTOs.Property;
using PropertyManagementAPI.Domain.DTOs.Users;
using PropertyManagementAPI.Infrastructure.Repositories.Invoices;
using PropertyManagementAPI.Infrastructure.Repositories.Property;
using PropertyManagementAPI.Infrastructure.Repositories.Tenants;

namespace PropertyManagementAPI.Application.Services.Quickbooks
{
    public class QuickBooksCustomerMapper : IQuickBooksCustomerMapper
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly ILogger<QuickBooksCustomerMapper> _logger;

        public QuickBooksCustomerMapper( ILogger<QuickBooksCustomerMapper> logger, ITenantRepository tenantRepository, IPropertyRepository propertyRepository)
        {
            _logger = logger;
            _tenantRepository = tenantRepository;
            _propertyRepository = propertyRepository;
        }

        public async Task<Customer> ToQuickBooksCustomer(int propertyId, TenantDto tenant)
        {
            PropertyAddressDto? properties = await _propertyRepository.GetPropertyByIdAsync(propertyId);
            if (properties == null)
            {
                _logger.LogError($"Property with ID {propertyId} not found.");
                throw new ArgumentException($"Property with ID {propertyId} not found.");
            }

            return new Customer
            {
                DisplayName = $"{tenant.FirstName?.Trim()} {tenant.LastName?.Trim()}".Trim(),
                PrimaryEmailAddr = new Intuit.Ipp.Data.EmailAddress
                {
                    Address = tenant.Email
                },
                BillAddr = new Intuit.Ipp.Data.PhysicalAddress
                {
                    Line1 = properties.Address,
                    Line2 = properties.Address1,
                    City = properties.City,
                    CountrySubDivisionCode = properties.State,
                    PostalCode = properties.PostalCode,
                    Country = "USA"
                }
            };
        }

    }
}
