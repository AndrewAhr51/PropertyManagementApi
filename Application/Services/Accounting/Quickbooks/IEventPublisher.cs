namespace PropertyManagementAPI.Application.Services.Accounting.Quickbooks
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent evt);
    }
}
