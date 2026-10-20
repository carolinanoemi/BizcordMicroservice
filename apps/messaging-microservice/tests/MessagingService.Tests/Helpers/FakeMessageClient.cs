using MessagingService.Messaging;

namespace MessagingService.Tests.Helpers;

/// <summary>
/// A fake version of the message client used in tests.
/// Instead of actually sending events to RabbitMQ, it stores them in a list
/// so we can check what was published. This means our tests do not need
/// a real RabbitMQ server running.
/// </summary>
public class FakeMessageClient : IMessageClient
{
    // Every event that gets published ends up in this list
    public List<object> Published { get; } = new();

    /// <summary>
    /// Instead of sending to RabbitMQ, just add the event to our list.
    /// </summary>
    public Task PublishAsync<T>(T message) where T : class
    {
        Published.Add(message!);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Subscribe does nothing in tests - we are only testing what gets published.
    /// </summary>
    public Task SubscribeAsync<T>(string subscriptionId, Func<T, Task> onMessage) where T : class
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Returns the single published event of a specific type.
    /// Useful for checking: did we publish exactly one MessagePostedEvent?
    /// </summary>
    public T? SinglePublished<T>() where T : class
    {
        return Published.OfType<T>().SingleOrDefault();
    }

    public void Dispose() { }
}