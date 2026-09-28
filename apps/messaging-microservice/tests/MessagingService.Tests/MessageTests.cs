using FluentAssertions;
using MessagingService.Domain.Entities;
using MessagingService.Domain.Exceptions;

namespace MessagingService.Tests;

/// <summary>
/// Unit tests for the Message entity.
/// These test our business rules — no HTTP, no database, no RabbitMQ.
/// Just pure logic: does Message.Create() do what it should?
/// </summary>
public class MessageTests
{
    /// <summary>
    /// When we create a message with valid data, all fields should be set correctly.
    /// </summary>
    [Fact]
    public void Message_Create_SetsAllFields()
    {
        // Arrange — set up test data
        var channelId = Guid.NewGuid();
        var authorId = Guid.NewGuid();

        // Act — call the method we're testing
        var message = Message.Create(channelId, authorId, "Hello world");

        // Assert — check the results using FluentAssertions
        // .Should().NotBe() reads like English: "message.Id should not be empty"
        message.Id.Should().NotBe(Guid.Empty);
        message.ChannelId.Should().Be(channelId);
        message.AuthorId.Should().Be(authorId);
        message.Content.Should().Be("Hello world");
        message.PostedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        message.IsDeleted.Should().BeFalse();
    }

    /// <summary>
    /// When we try to create a message with empty content, it should throw a DomainException.
    /// This tests our validation rule inside Message.Create().
    /// </summary>
    [Fact]
    public void Message_Create_Throws_WhenContentIsEmpty()
    {
        // Act — try to create a message with empty content
        var act = () => Message.Create(Guid.NewGuid(), Guid.NewGuid(), "");

        // Assert — it should throw our custom DomainException
        act.Should().Throw<DomainException>();
    }

    /// <summary>
    /// Whitespace-only content should also be rejected.
    /// </summary>
    [Fact]
    public void Message_Create_Throws_WhenContentIsWhitespace()
    {
        var act = () => Message.Create(Guid.NewGuid(), Guid.NewGuid(), "   ");

        act.Should().Throw<DomainException>();
    }
}