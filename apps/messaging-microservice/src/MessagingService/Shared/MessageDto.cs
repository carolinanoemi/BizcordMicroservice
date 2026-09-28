namespace MessagingService.Shared;

/// <summary>
/// The public-facing model that other services see when they call our API.
/// This hides internal fields like IsDeleted and EditedAt.
/// </summary>
public class MessageDto
{
    // Changed from Id to MessageId to match what the assignment's
    // integration test expects (body.MessageId)
    public Guid MessageId { get; set; }
    public Guid ChannelId { get; set; }
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}