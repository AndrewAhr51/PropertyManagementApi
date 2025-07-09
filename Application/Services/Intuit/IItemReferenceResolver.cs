namespace PropertyManagementAPI.Application.Services.Intuit
{
    public interface IItemReferenceResolver
    {
        string? ResolveItemId(string itemTypeName);
    }


}
