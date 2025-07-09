namespace PropertyManagementAPI.Application.Services.Accounting.Quickbooks
{
    public interface ITokenEncryptionService
    {
        string Encrypt(string plain);
        string Decrypt(string cipher);
    }
}
