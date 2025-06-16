using Microsoft.Extensions.Options;
using PropertyManagementAPI.Common.Utilities;
using System.Security.Cryptography;
using System.Text;

public class EncryptionHelper
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public EncryptionHelper(IOptions<EncryptionSettings> options)
    {
        var settings = options.Value;

        if (settings.Key.Length != 32 || settings.IV.Length != 16)
            throw new ArgumentException("Key must be 32 bytes and IV must be 16 bytes for AES-256.");

        // Debugging: Check actual lengths
        Console.WriteLine($"Key Length: {settings.Key.Length}, IV Length: {settings.IV.Length}");

        _key = Encoding.UTF8.GetBytes(settings.Key);
        _iv = Encoding.UTF8.GetBytes(settings.IV);
    }

    public string Encrypt(string plaintext)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        var bytes = Encoding.UTF8.GetBytes(plaintext);

        return Convert.ToBase64String(encryptor.TransformFinalBlock(bytes, 0, bytes.Length));
    }

    public string Decrypt(string encryptedText)
    {
        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        var bytes = Convert.FromBase64String(encryptedText);

        return Encoding.UTF8.GetString(decryptor.TransformFinalBlock(bytes, 0, bytes.Length));
    }
}