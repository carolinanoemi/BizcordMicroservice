using MessagingService.Domain.Exceptions;

namespace MessagingService.Domain.Entities;

/// <summary>
/// The core entity of our microservice, a chat message.
/// This is our INTERNAL model. Other services don't see this directly,
/// they only see the shared MessageDto or MessagePostedEvent.
/// </summary>
public class Message
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid ChannelId { get; set; }

    // Renamed from SenderId to AuthorId to match the shared contract
    // that the whole class agreed on (MessagePostedEvent uses AuthorId)
    public Guid AuthorId { get; set; }

    // Renamed from CreatedAt to PostedAt to match the shared contract
    public DateTime PostedAt { get; set; }
    public DateTime? EditedAt { get; set; }
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Factory method that creates a new Message with validation.
    /// We use this instead of "new Message()" so the validation logic
    /// lives inside the entity itself — not scattered across services.
    /// If the content is empty or whitespace, it throws a DomainException.
    /// </summary>
    public static Message Create(Guid channelId, Guid authorId, string content)
    {
        // Validate: don't allow empty messages
        if (string.IsNullOrWhiteSpace(content))
            throw new DomainException("Message content cannot be empty.");

        return new Message
        {
            Id = Guid.NewGuid(),
            ChannelId = channelId,
            AuthorId = authorId,
            Content = content,
            PostedAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }
}