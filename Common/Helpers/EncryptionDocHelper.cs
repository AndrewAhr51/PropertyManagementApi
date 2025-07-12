using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace PropertyManagementAPI.Common.Helpers
{
    public class EncryptionDocHelper
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;
        private readonly ILogger<EncryptionDocHelper> _logger;

        public EncryptionDocHelper(IOptions<EncryptionDocSettings> options, ILogger<EncryptionDocHelper> logger)
        {
            var settings = options.Value;
            _logger = logger;

            _key = Convert.FromBase64String(settings.Key);
            _iv = Convert.FromBase64String(settings.IV);

            if (_key.Length != 32 || _iv.Length != 16)
            {
                _logger.LogError("Invalid AES key or IV length: Key({KeyLength}), IV({IVLength})", _key.Length, _iv.Length);
                throw new ArgumentException("Key must be 32 bytes and IV must be 16 bytes for AES-256.");
            }

            _logger.LogInformation("Encryption helper initialized with valid AES-256 key and IV.");
        }

        public byte[] EncryptBytes(byte[] content)
        {
            try
            {
                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var encryptor = aes.CreateEncryptor();
                using var ms = new MemoryStream();
                using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);

                cs.Write(content, 0, content.Length);
                cs.FlushFinalBlock();

                _logger.LogInformation("Encryption completed successfully. Original size: {OriginalSize}, Encrypted size: {EncryptedSize}", content.Length, ms.Length);
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to encrypt content.");
                throw;
            }
        }

        public byte[] DecryptBytes(byte[] encryptedContent)
        {
            try
            {
                using var aes = Aes.Create();
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var decryptor = aes.CreateDecryptor();
                using var ms = new MemoryStream(encryptedContent);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);

                using var output = new MemoryStream();
                cs.CopyTo(output);

                _logger.LogInformation("Decryption completed successfully. Encrypted size: {EncryptedSize}, Decrypted size: {DecryptedSize}", encryptedContent.Length, output.Length);
                return output.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to decrypt content.");
                throw;
            }
        }
    }
}