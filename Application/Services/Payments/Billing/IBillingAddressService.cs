using PropertyManagementAPI.Domain.DTOs.Payments.Billing;
using PropertyManagementAPI.Domain.Entities.Payments.Billing;

namespace PropertyManagementAPI.Application.Services.Payments.Billing
{
    public interface IBillingAddressService
    {
        Task<int> AddBillingAddressAsync(BillingAddressDto dto);
        Task UpdateBillingAddressAsync(int billingAddressId, BillingAddressDto dto);
        Task<IEnumerable<BillingAddressHistory>> GetAddressHistoryAsync(int billingAddressId);
    }

}
