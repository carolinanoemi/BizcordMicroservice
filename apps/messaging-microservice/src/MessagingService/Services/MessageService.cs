using MessagingService.Domain.Entities;
using MessagingService.Domain.Events;
using MessagingService.DTOs;
using MessagingService.Messaging;
using MessagingService.Repositories;

namespace MessagingService.Services;

/// <summary>
/// This class contains all the business logic for messages.
/// It sits between the controller (which handles HTTP requests) 
/// and the repository (which handles saving/loading data).
/// It also uses the message client to publish events to RabbitMQ,
/// so other services like the Notification Service can react.
/// </summary>
public class MessageService : IMessageService
{
    private readonly IMessageRepository _repository;
    private readonly IMessageClient _messageClient;

    /// <summary>
    /// .NET automatically gives us these two dependencies (dependency injection).
    /// We registered them in Program.cs — we never create them with "new" ourselves.
    /// _repository = where we save/load messages (currently an in-memory list)
    /// _messageClient = our RabbitMQ connection for sending events to other services
    /// </summary>
    public MessageService(IMessageRepository repository, IMessageClient messageClient)
    {
        _repository = repository;
        _messageClient = messageClient;
    }

    /// <summary>
    /// Find one message by its ID. Returns null if it doesn't exist.
    /// </summary>
    public async Task<Message?> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    /// <summary>
    /// Get all messages in a specific channel, sorted oldest to newest.
    /// </summary>
    public async Task<List<Message>> GetByChannelIdAsync(Guid channelId)
    {
        return await _repository.GetByChannelIdAsync(channelId);
    }

    /// <summary>
    /// Create a new message and notify other services about it.
    /// </summary>
    public async Task<Message> CreateAsync(CreateMessageRequest request)
    {
        // Step 1: Build a Message entity from the incoming request data
        var message = new Message
        {
            Id = Guid.NewGuid(),
            Content = request.Content,
            ChannelId = request.ChannelId,
            SenderId = request.SenderId,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        // Step 2: Save the message to the database via the repository
        await _repository.AddAsync(message);

        // Step 3: Use the message client to publish a "MessageSent" event to RabbitMQ.
        // The Notification Service listens for this event and sends push notifications
        // to users in the channel. Our service doesn't call Notification Service directly —
        // it just publishes the event, and any service that cares will pick it up.
        await _messageClient.PublishAsync(new MessageSent
        {
            MessageId = message.Id,
            ChannelId = message.ChannelId,
            SenderId = message.SenderId,
            Content = message.Content,
            SentAt = message.CreatedAt
        });

        return message;
    }

    /// <summary>
    /// Update an existing message's content and notify other services.
    /// Returns null if the message doesn't exist.
    /// </summary>
    public async Task<Message?> UpdateAsync(Guid id, UpdateMessageRequest request)
    {
        // Step 1: Find the existing message in the database
        var message = await _repository.GetByIdAsync(id);
        if (message == null)
            return null; // Message not found — controller will return 404

        // Step 2: Update the fields with the new data
        message.Content = request.Content;
        message.EditedAt = DateTime.UtcNow;

        // Step 3: Save the changes to the database
        await _repository.UpdateAsync(message);

        // Step 4: Use the message client to publish a "MessageEdited" event to RabbitMQ.
        // The Engagement Service listens for this and can update any previews or cached
        // versions of this message in other parts of the system.
        await _messageClient.PublishAsync(new MessageEdited
        {
            MessageId = message.Id,
            NewContent = message.Content,
            EditedAt = message.EditedAt.Value
        });

        return message;
    }

    /// <summary>
    /// Soft-delete a message (marks it as deleted, doesn't remove it from the database).
    /// Returns false if the message doesn't exist.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        // Step 1: Check if the message exists
        var message = await _repository.GetByIdAsync(id);
        if (message == null)
            return false; // Message not found — controller will return 404

        // Step 2: Soft delete — sets IsDeleted = true in the database.
        // We keep the data but hide it from queries, instead of permanently removing it.
        await _repository.DeleteAsync(id);

        // Step 3: Use the message client to publish a "MessageDeleted" event to RabbitMQ.
        // The Notification Service and Engagement Service listen for this, so they can
        // clean up any notifications or reactions tied to this message.
        await _messageClient.PublishAsync(new MessageDeleted
        {
            MessageId = message.Id,
            ChannelId = message.ChannelId,
            DeletedAt = DateTime.UtcNow
        });

        return true;
    }
}