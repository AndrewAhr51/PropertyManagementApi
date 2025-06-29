using PropertyManagementAPI.Application.Services.Payments;
using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.Entities.Payments;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments
{
    public interface IPaymentRepository
    {
        Task<Payment> CreatePaymentAsync(CreatePaymentDto dto);
        Task<PayPalPaymentResponseDto> CreatePayPalPaymentAsync(CreatePayPalDto dto);
        Task<Payment> GetPaymentByIdAsync(int paymentId);
        Task<IEnumerable<Payment>> GetPaymentByInvoiceIdAsync(int invoiceId);
        Task AddPaymentAsync(Payment payment);
        Task SavePaymentChangesAsync();
    }

}
