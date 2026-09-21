namespace MessagingService.Messaging;

public interface IMessageClient : IDisposable
{
    Task PublishAsync<T>(T message) where T : class;

    Task SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage) where T : class;
}