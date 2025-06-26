using AutoMapper;
using PropertyManagementAPI.Domain.DTOs.Payments;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Infrastructure.Repositories.Payments;

namespace PropertyManagementAPI.Application.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;

        public PaymentService(IPaymentRepository paymentRepo)
        {
            _paymentRepo = paymentRepo;
        }

        public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto)
        {
            var payment = new Payment
            {
                InvoiceId = dto.InvoiceId,
                TenantId = dto.TenantId,
                PropertyId = dto.PropertyId,
                Amount = dto.Amount,
                PaymentMethodId = dto.PaymentMethodId,
                Status = dto.Status,
                Notes = dto.Notes,
                ReferenceNumber = dto.ReferenceNumber,
                CreatedBy = dto.CreatedBy ?? "Web",
                TransactionDate = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow
            };

            await _paymentRepo.CreatePaymentAsync(payment);
            await _paymentRepo.SavePaymentChangesAsync();

            return new PaymentDto
            {
                PaymentId = payment.PaymentId,
                InvoiceId = payment.InvoiceId,
                TenantId = payment.TenantId,
                PropertyId = payment.PropertyId,
                Amount = payment.Amount,
                PaymentMethodId = payment.PaymentMethodId,
                Status = payment.Status,
                Notes = payment.Notes,
                ReferenceNumber = payment.ReferenceNumber,
                CreatedBy = payment.CreatedBy,
                TransactionDate = payment.TransactionDate,
                CreatedDate = payment.CreatedDate
            };
        }

        public async Task<PaymentDto?> GetPaymentAsync(int paymentId)
        {
            var payment = await _paymentRepo.GetPaymentByIdAsync(paymentId);
            if (payment is null) return null;

            return new PaymentDto
            {
                PaymentId = payment.PaymentId,
                InvoiceId = payment.InvoiceId,
                TenantId = payment.TenantId,
                PropertyId = payment.PropertyId,
                Amount = payment.Amount,
                PaymentMethodId = payment.PaymentMethodId,
                Status = payment.Status,
                Notes = payment.Notes,
                ReferenceNumber = payment.ReferenceNumber,
                CreatedBy = payment.CreatedBy,
                TransactionDate = payment.TransactionDate,
                CreatedDate = payment.CreatedDate
            };
        }

        public async Task<IEnumerable<PaymentDto>> GetPaymentsByTenantAsync(int tenantId)
        {
            var payments = await _paymentRepo.GetPaymentByTenantIdAsync(tenantId);

            return payments.Select(p => new PaymentDto
            {
                PaymentId = p.PaymentId,
                InvoiceId = p.InvoiceId,
                TenantId = p.TenantId,
                PropertyId = p.PropertyId,
                Amount = p.Amount,
                PaymentMethodId = p.PaymentMethodId,
                Status = p.Status,
                Notes = p.Notes,
                ReferenceNumber = p.ReferenceNumber,
                CreatedBy = p.CreatedBy,
                TransactionDate = p.TransactionDate,
                CreatedDate = p.CreatedDate
            });
        }
    }
}
