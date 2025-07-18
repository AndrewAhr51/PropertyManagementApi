namespace PropertyManagementAPI.Common.Helpers
{
    public static class RedirectUrlHelper
    {
        public static string GetSuccessUrl(string sessionId, string env)
        {
            var baseUrl = env switch
            {
                "Production" => "https://app.omnitenant.com",
                "Staging" => "https://staging.omnitenant.com",
                _ => "http://localhost:4200"
            };

            return $"{baseUrl}/payment-success?session_id={sessionId}";
        }
    }

}
