namespace PropertyManagementAPI.Application.Services.Accounting.Quickbooks
{
    public interface IQuickBooksUrlService
    {
        string GetAuthorizationUrl(int tenantId);
    }
}
