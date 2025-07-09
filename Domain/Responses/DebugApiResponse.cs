namespace PropertyManagementAPI.Domain.Responses
{
    public class DebugApiResponse
    {
        public string Message { get; set; }
        public object? Payload { get; set; }

        public DebugApiResponse(string message, object? payload = null)
        {
            Message = message;
            Payload = payload;
        }
    }
}