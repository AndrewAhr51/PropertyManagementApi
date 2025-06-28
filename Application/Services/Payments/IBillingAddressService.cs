using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.Entities.Payments;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public interface IBillingAddressService
    {
        Task<int> AddBillingAddressAsync(BillingAddressDto dto);
        Task UpdateBillingAddressAsync(int billingAddressId, BillingAddressDto dto);
        Task<IEnumerable<BillingAddressHistory>> GetAddressHistoryAsync(int billingAddressId);
    }

}
