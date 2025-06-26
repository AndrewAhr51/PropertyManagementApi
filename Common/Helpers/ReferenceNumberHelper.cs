using Microsoft.AspNetCore.Mvc;

namespace PropertyManagementAPI.Common.Helpers
{
    public static class ReferenceNumberHelper
    {
        public static string Generate(string prefix, int propertyId = 0)
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            var random = new Random().Next(1000, 9999);
            return $"{prefix}-{propertyId:D5}-{timestamp}-{random}";
        }
    }
}
