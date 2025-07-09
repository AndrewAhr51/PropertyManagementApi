namespace PropertyManagementAPI.Application.Services.Intuit
{
    public class DefaultItemReferenceResolver : IItemReferenceResolver
    {
        public string? ResolveItemId(string itemTypeName)
        {
            // Replace with DB lookup or config binding
            return itemTypeName switch
            {
                "Rent" => "2001",
                "Service" => "1234",
                _ => "default"
            };
        }
    }

}
