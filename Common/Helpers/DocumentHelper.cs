using System.Security.Cryptography;

namespace PropertyManagementAPI.Common.Helpers
{
    public class DocumentHelper
    {
       public static string GetChecksum(byte[] content)
        {
            if (content == null || content.Length == 0)
            {
                throw new ArgumentException("Content cannot be null or empty.", nameof(content));
            }
            return GenerateChecksum(content);
        }

        private static string GenerateChecksum(byte[] content)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(content);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
