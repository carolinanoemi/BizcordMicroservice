namespace MessagingService.Domain.Events;

/// <summary>
/// NOTE: This was our original internal event. We're keeping it for now,
/// but the shared contract (MessagePostedEvent) is what other services
/// actually listen for. This could be removed later if we don't need it.
/// </summary>
public class MessageSent
{
    public Guid MessageId { get; set; }
    public Guid ChannelId { get; set; }
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}