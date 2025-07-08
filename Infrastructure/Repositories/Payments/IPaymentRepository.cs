using PropertyManagementAPI.Application.Services.Payments;
using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.DTOs.Payments.PayPal;
using PropertyManagementAPI.Domain.DTOs.Payments.Stripe;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Domain.Entities.Payments.CreditCard;
using System.Threading.Tasks;
using Stripe = Stripe.Invoice;

namespace PropertyManagementAPI.Infrastructure.Repositories.Payments
{
    public interface IPaymentRepository
    {
        Task<Payment> ProcessPaymentAsync(CreatePaymentDto dto);
        Task<Payment> GetPaymentByIdAsync(int paymentId);
        Task<IEnumerable<Payment>> GetPaymentByInvoiceIdAsync(int invoiceId);
        Task AddPaymentAsync(Payment payment);
        Task SavePaymentChangesAsync();
        Task<bool> ReversePaymentAsync(int paymentId);
    }
}


