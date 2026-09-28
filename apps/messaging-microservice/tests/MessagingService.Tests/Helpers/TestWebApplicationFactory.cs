using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MessagingService.Messaging;

namespace MessagingService.Tests.Helpers;

/// <summary>
/// This creates a test version of our entire app.
/// It is like running "dotnet run" but in memory, inside the test.
/// 
/// The key difference: we swap out the real RabbitMQ message client
/// for our FakeMessageClient, so tests do not need a real RabbitMQ server.
/// 
/// The test can then use HttpClient to call our API endpoints
/// (like POST /api/messages) just like a real client would.
/// </summary>
public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    // We keep a reference to the fake client so tests can inspect what was published
    public FakeMessageClient MessageClient { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the real RabbitMQ message client that Program.cs registered
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IMessageClient));
            if (descriptor != null)
                services.Remove(descriptor);

            // Replace it with our fake that just captures published events
            services.AddSingleton<IMessageClient>(MessageClient);
        });
    }
}