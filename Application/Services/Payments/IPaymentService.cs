using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.Entities.Payments;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(CreatePaymentDto dto);
        Task<Payment> GetPaymentByIdAsync(int paymentId);
        Task<IEnumerable<Payment>> GetPaymentsByInvoiceIdAsync(int invoiceId);
    }

}
