namespace PropertyManagementAPI.Common.Helpers
{
    public class AuditEventBuilder
    {
        private readonly PaymentAuditLogger _auditLogger;

        public AuditEventBuilder(PaymentAuditLogger auditLogger)
        {
            _auditLogger = auditLogger;
        }

        public async Task LogSuccessAsync(string action, object payload, string performedBy = "System")
        {
            await _auditLogger.LogAsync(
                action: action,
                status: "SUCCESS",
                response: payload,
                performedBy: performedBy
            );
        }

        public async Task LogFailureAsync(string action, object payload, string performedBy = "System")
        {
            await _auditLogger.LogAsync(
                action: action,
                status: "FAILED",
                response: payload,
                performedBy: performedBy
            );
        }
    }

}
