using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace PropertyManagementAPI.Common.Helpers
{
    public class EncryptionDocHelper
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public EncryptionDocHelper(IOptions<EncryptionDocSettings> options)
        {
            var settings = options.Value;
            _key = Convert.FromBase64String(settings.Key);
            _iv = Convert.FromBase64String(settings.IV);

            if (_key.Length != 32 || _iv.Length != 16)
                throw new ArgumentException("Key must be 32 bytes and IV must be 16 bytes for AES-256.");
        }

        public byte[] EncryptBytes(byte[] content)
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

            return ms.ToArray();
        }

        public byte[] DecryptBytes(byte[] encryptedContent)
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

            return output.ToArray();
        }
    }
}
