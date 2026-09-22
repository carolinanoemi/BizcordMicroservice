namespace MessagingService.Domain.Events;

public class MessageEdited
{
    public Guid MessageId { get; set; }
    public string NewContent { get; set; } = string.Empty;
    public DateTime EditedAt { get; set; }
}