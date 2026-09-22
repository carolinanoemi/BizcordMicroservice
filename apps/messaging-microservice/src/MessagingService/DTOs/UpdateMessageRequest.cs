using System.ComponentModel.DataAnnotations;

namespace MessagingService.DTOs;

public class UpdateMessageRequest
{
    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Content { get; set; } = string.Empty;
}