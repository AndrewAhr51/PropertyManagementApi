using PropertyManagementAPI.Application.Services.Accounting.Quickbooks;
using PropertyManagementAPI.Application.Services.Quickbooks;
using PropertyManagementAPI.Infrastructure.Quickbooks;
using PropertyManagementAPI.Infrastructure.Repositories.Property;
using PropertyManagementAPI.Infrastructure.Repositories.Tenants;

namespace PropertyManagementAPI.Application.Services.Tenants
{
    public class TenantOnboardingService: ITenantOnboardingService
    {
        private readonly QuickBooksInvoiceService _invoiceService;
        private readonly ITenantRepository _tenantRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IQuickBooksCustomerMapper _customerMapper;

       public TenantOnboardingService(QuickBooksInvoiceService invoiceService,ITenantRepository tenantRepository, IPropertyRepository propertyRepository, IQuickBooksCustomerMapper customerMapper)
        {
            _invoiceService = invoiceService;
            _tenantRepository = tenantRepository;
            _propertyRepository = propertyRepository;
            _customerMapper = customerMapper;
        }
        public async Task OnPlaidAccountVerified(int propertyId, int tenantId)
        {
            // Retrieve property information
            var property = await _propertyRepository.GetPropertyByIdAsync(propertyId);
            if (property == null) throw new Exception($"property {propertyId} not found.");
            
            // Validate property has a monthly rent set
            if (property.Amount == null || property.Amount <= 0)
                throw new Exception($"Property {propertyId} does not have a valid monthly rent set.");
            
            // Retrieve tenant information
            var tenant = await _tenantRepository.GetTenantByIdAsync(tenantId);
            if (tenant == null) throw new Exception($"Tenant {tenantId} not found.");

            var customer = await _customerMapper.ToQuickBooksCustomer(propertyId, tenant);
            if (customer == null) throw new Exception($"Failed to map tenant {tenantId} to QuickBooks customer.");
            
            // Create invoice in QuickBooks for the tenant's first month rent
            // Example: realmId stored in settings or linked to property management company
            var realmId = tenant.RealmId;
            var itemId = "1"; // e.g., "Rent" item in QBO
            var Amount = property.Amount;

            var invoice = await _invoiceService.CreateInvoiceAsync(realmId, customer, (decimal)Amount, itemId);

            // Optional: mark onboarding complete or send confirmation email
            // await _notificationService.SendInvoiceCreated(tenant.Email, invoice);
        }

    }
}
