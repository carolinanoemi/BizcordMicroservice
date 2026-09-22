using System.ComponentModel.DataAnnotations;

namespace MessagingService.DTOs;

public class CreateMessageRequest
{
    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;

    [Required]
    public Guid ChannelId { get; set; }

    [Required]
    public Guid SenderId { get; set; }
}