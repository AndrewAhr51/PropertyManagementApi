using Intuit.Ipp.Data;
using PropertyManagementAPI.Domain.Entities.User;
using PropertyManagementAPI.Domain.DTOs.Users;

namespace PropertyManagementAPI.Application.Services.Quickbooks
{
    public interface IQuickBooksCustomerMapper
    {
        Task <Customer> ToQuickBooksCustomer(int propertyId, TenantDto tenant);
    }
}

