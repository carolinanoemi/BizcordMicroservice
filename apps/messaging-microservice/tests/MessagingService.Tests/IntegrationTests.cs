using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MessagingService.Shared;
using MessagingService.Tests.Helpers;

namespace MessagingService.Tests;

/// <summary>
/// Integration tests.
/// These test the full round trip: HTTP request, controller, service, repository, HTTP response.
/// They use the real app running in memory (via TestWebApplicationFactory),
/// but with a fake RabbitMQ client so we do not need a real broker.
/// </summary>
public class IntegrationTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public IntegrationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    /// <summary>
    /// Full round trip: POST a message, check we get 201 Created,
    /// then GET the same message and check it was actually saved.
    /// </summary>
    [Fact]
    public async Task PostMessage_Returns201_AndPersistsMessage()
    {
        // Arrange and Act - create a new message via the API
        var response = await _client.PostAsJsonAsync("/api/messages", new
        {
            ChannelId = Guid.NewGuid(),
            AuthorId = Guid.NewGuid(),
            Content = "Hello world"
        });

        // Assert - we should get 201 Created
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Read the response body as a MessageDto
        var body = await response.Content.ReadFromJsonAsync<MessageDto>();
        body.Should().NotBeNull();
        body!.MessageId.Should().NotBe(Guid.Empty);

        // Verify it was actually persisted - GET the same message by its ID
        var get = await _client.GetAsync($"/api/messages/{body.MessageId}");
        get.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}