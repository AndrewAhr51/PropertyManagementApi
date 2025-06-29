
using Microsoft.EntityFrameworkCore;
using PropertyManagementAPI.Domain.Entities.Payments;
using PropertyManagementAPI.Infrastructure.Data;
using System.Text.Json;

namespace PropertyManagementAPI.Common.Helpers
{
    public class PaymentAuditLogger
    {
        private readonly MySqlDbContext _dbContext;

        public PaymentAuditLogger(MySqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task LogAsync(string action, string status, object response, int? paymentId = null, string? performedBy = null)
        {
            var log = new PaymentAuditLog
            {
                PaymentId = paymentId,
                Gateway = "PayPal",
                Action = action,
                Status = status,
                ResponsePayload = JsonSerializer.Serialize(response),
                Timestamp = DateTime.UtcNow,
                PerformedBy = performedBy ?? "System"
            };

            _dbContext.PaymentAuditLogs.Add(log);
            await _dbContext.SaveChangesAsync();
        }
    }
}
