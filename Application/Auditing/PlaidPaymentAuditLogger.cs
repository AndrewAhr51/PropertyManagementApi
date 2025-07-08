namespace PropertyManagementAPI.Infrastructure.Auditing
{
    public class PlaidPaymentAuditLogger
    {
        private readonly ILogger<PlaidPaymentAuditLogger> _logger;

        public PlaidPaymentAuditLogger(ILogger<PlaidPaymentAuditLogger> logger)
        {
            _logger = logger;
        }

        public Task LogPlaidSuccessAsync(string operation, string message, string correlationId)
        {
            _logger.LogInformation("✅ [Plaid][{Operation}] Success | Correlation: {CorrelationId} | {Message}",
                operation, correlationId, message);
            return Task.CompletedTask;
        }

        public Task LogPlaidFailureAsync(string operation, object? errorDetail, string correlationId)
        {
            _logger.LogError("❌ [Plaid][{Operation}] Failure | Correlation: {CorrelationId} | Detail: {@ErrorDetail}",
                operation, correlationId, errorDetail);
            return Task.CompletedTask;
        }
    }
}