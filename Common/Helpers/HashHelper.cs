using System.Security.Cryptography;
using System.Text;

namespace PropertyManagementAPI.Common.Helpers
{
    public static class HashHelper
    {
        /// <summary>
        /// Generates a SHA256 hash from a plain string input.
        /// </summary>
        public static string Sha256(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);

            var builder = new StringBuilder();
            foreach (var b in hash)
                builder.Append(b.ToString("x2")); // Hex format

            return builder.ToString();
        }
    }
}