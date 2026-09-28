using System.ComponentModel.DataAnnotations;

namespace MessagingService.DTOs;

/// <summary>
/// The data the API expects when someone creates a new message.
/// [Required] and [StringLength] are validation rules —
/// .NET checks these automatically and returns 400 Bad Request if they fail.
/// </summary>
public class CreateMessageRequest
{
    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;

    [Required]
    public Guid ChannelId { get; set; }

    // Renamed from SenderId to AuthorId to match the shared contract
    [Required]
    public Guid AuthorId { get; set; }
}