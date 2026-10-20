using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MessagingService.Tests.Helpers;
using Shared.Contracts.Events;

namespace MessagingService.Tests;

/// <summary>
/// Provider contract tests.
/// These verify that when we publish a MessagePostedEvent to RabbitMQ,
/// it contains all the fields other teams depend on, with correct values.
/// 
/// This is the most important test in the project. If we break this,
/// every other team's microservice breaks too.
/// </summary>
public class ContractTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestWebApplicationFactory _factory;

    public ContractTests(TestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
        // Clear any events from previous tests
        _factory.MessageClient.Published.Clear();
    }

    /// <summary>
    /// When we POST a new message, the MessagePostedEvent we publish to RabbitMQ
    /// must contain every field that other services depend on, with correct values.
    /// 
    /// This is our PROMISE to other teams:
    /// "If you listen for MessagePostedEvent, these fields will always be there."
    /// </summary>
    [Fact]
    public async Task PostMessage_PublishedEvent_MeetsContract()
    {
        // Arrange - set up test data
        var channelId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        // Act - call our POST /api/messages endpoint
        var response = await _client.PostAsJsonAsync("/api/messages", new
        {
            ChannelId = channelId,
            AuthorId = authorId,
            Content = "Hello world"
        });

        // Assert - check the HTTP response
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Assert - check the event that was published to RabbitMQ (via FakeMessageClient)
        var evt = _factory.MessageClient.SinglePublished<MessagePostedEvent>();

        // Every field another service reads must be asserted here
        evt.Should().NotBeNull();
        evt!.MessageId.Should().NotBe(Guid.Empty);
        evt.ChannelId.Should().Be(channelId);
        evt.AuthorId.Should().Be(authorId);
        evt.Content.Should().Be("Hello world");
        evt.PostedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}