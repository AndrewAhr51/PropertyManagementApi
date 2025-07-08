using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.DTOs.Payments.PayPal;
using PropertyManagementAPI.Domain.DTOs.Payments.Stripe;
using PropertyManagementAPI.Domain.Entities.Invoices;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public interface IPaymentService
    {
        // 📦 General payment operations
        Task<Payment> ProcessPaymentAsync(CreatePaymentDto dto);
        Task<bool> ReversePaymentAsync(int paymentId);
        Task<Payment> GetPaymentByIdAsync(int paymentId);
        Task<IEnumerable<Payment>> GetPaymentsByInvoiceIdAsync(int invoiceId);
        Task<Invoice?> GetInvoiceAsync(int invoiceId);


    }

}