using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Domain.Entities.Payments;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public interface IPaymentProcessor
    {   
        Task<string> CreatePayPalOrderAsync(decimal amount, string currency, Invoice invoice);
        Task<string> CapturePayPalOrderAsync(string orderId);
        Task<string> GetApprovalLinkAsync(string orderId);
        

    }

}
