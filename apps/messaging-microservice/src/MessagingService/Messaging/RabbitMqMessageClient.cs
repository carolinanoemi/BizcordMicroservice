using EasyNetQ;

namespace MessagingService.Messaging;

public class RabbitMqMessageClient : IMessageClient
{
    private readonly IBus _bus;

    public RabbitMqMessageClient(IBus bus)
    {
        _bus = bus;
    }

    public async Task PublishAsync<T>(T message) where T : class
    {
        await _bus.PubSub.PublishAsync(message);
    }

    public async Task SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage) where T : class
    {
        await _bus.PubSub.SubscribeAsync<T>(subscriptionId, onMessage);
    }

    public void Dispose()
    {
    }
}