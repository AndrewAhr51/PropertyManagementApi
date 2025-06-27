using Microsoft.AspNetCore.Mvc;

namespace PropertyManagementAPI.Common.Helpers
{
    public static class ReferenceNumberHelper
    {
        public static string Generate(string prefix, int propertyId = 0)
        {
            var random = new Random().Next(10000, 99999);
            return $"{prefix}-{propertyId:D5}-{random}";
        }
    }
}
