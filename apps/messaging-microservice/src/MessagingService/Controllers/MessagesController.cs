using Microsoft.AspNetCore.Mvc;
using MessagingService.DTOs;
using MessagingService.Services;
using MessagingService.Shared;

namespace MessagingService.Controllers;

/// <summary>
/// This is the REST API controller for messages.
/// It handles HTTP requests (GET, POST, PUT, DELETE) and returns HTTP responses.
/// It does NOT contain any business logic — it just passes work to the MessageService.
/// 
/// The route "api/messages" means all endpoints start with: http://localhost:5280/api/messages
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    /// <summary>
    /// .NET automatically gives us the MessageService (dependency injection).
    /// We registered it in Program.cs.
    /// </summary>
    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    /// <summary>
    /// GET api/messages/{id}
    /// Get a single message by its ID.
    /// Returns 200 OK with the message, or 404 Not Found if it doesn't exist.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var message = await _messageService.GetByIdAsync(id);
        if (message == null)
            return NotFound(); // 404 — message doesn't exist

        // Convert the internal entity to the shared model before returning.
        // We don't expose internal fields like IsDeleted or EditedAt to the outside.
        var response = new MessageDto
        {
            Id = message.Id,
            ChannelId = message.ChannelId,
            SenderId = message.SenderId,
            Content = message.Content,
            Timestamp = message.CreatedAt
        };

        return Ok(response); // 200 — here's your message
    }

    /// <summary>
    /// GET api/messages/channel/{channelId}
    /// Get all messages in a specific channel.
    /// Returns 200 OK with a list of messages (can be empty).
    /// </summary>
    [HttpGet("channel/{channelId}")]
    public async Task<IActionResult> GetByChannelId(Guid channelId)
    {
        var messages = await _messageService.GetByChannelIdAsync(channelId);

        // Convert each internal entity to the shared model
        var response = messages.Select(m => new MessageDto
        {
            Id = m.Id,
            ChannelId = m.ChannelId,
            SenderId = m.SenderId,
            Content = m.Content,
            Timestamp = m.CreatedAt
        }).ToList();

        return Ok(response); // 200 — here's all the messages
    }

    /// <summary>
    /// POST api/messages
    /// Create a new message. The request body must contain Content, ChannelId, and SenderId.
    /// Returns 201 Created with the new message, or 400 Bad Request if validation fails
    /// (e.g. empty content, missing fields — handled automatically by [Required] annotations on the DTO).
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMessageRequest request)
    {
        var message = await _messageService.CreateAsync(request);

        // Convert to shared model for the response
        var response = new MessageDto
        {
            Id = message.Id,
            ChannelId = message.ChannelId,
            SenderId = message.SenderId,
            Content = message.Content,
            Timestamp = message.CreatedAt
        };

        // 201 Created — includes a Location header pointing to GET api/messages/{id}
        return CreatedAtAction(nameof(GetById), new { id = message.Id }, response);
    }

    /// <summary>
    /// PUT api/messages/{id}
    /// Update an existing message's content.
    /// Returns 200 OK with the updated message, 404 if not found, or 400 if validation fails.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMessageRequest request)
    {
        var message = await _messageService.UpdateAsync(id, request);
        if (message == null)
            return NotFound(); // 404 — message doesn't exist

        var response = new MessageDto
        {
            Id = message.Id,
            ChannelId = message.ChannelId,
            SenderId = message.SenderId,
            Content = message.Content,
            Timestamp = message.CreatedAt
        };

        return Ok(response); // 200 — here's the updated message
    }

    /// <summary>
    /// DELETE api/messages/{id}
    /// Soft-delete a message (marks it as deleted, doesn't permanently remove it).
    /// Returns 204 No Content on success, or 404 if not found.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _messageService.DeleteAsync(id);
        if (!deleted)
            return NotFound(); // 404 — message doesn't exist

        return NoContent(); // 204 — deleted successfully, nothing to return
    }
}