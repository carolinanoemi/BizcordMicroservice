namespace MessagingService.Domain.Events;

public class MessageDeleted
{
    public Guid MessageId { get; set; }
    public Guid ChannelId { get; set; }
    public DateTime DeletedAt { get; set; }
}
