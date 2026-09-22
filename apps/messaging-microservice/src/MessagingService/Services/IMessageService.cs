using MessagingService.Domain.Entities;
using MessagingService.DTOs;

namespace MessagingService.Services;

/// <summary>
/// Defines the business logic for managing messages.
/// The controller calls these methods, it never touches the repository directly.
/// </summary>
public interface IMessageService
{
    Task<Message?> GetByIdAsync(Guid id);
    Task<List<Message>> GetByChannelIdAsync(Guid channelId);
    Task<Message> CreateAsync(CreateMessageRequest request);
    Task<Message?> UpdateAsync(Guid id, UpdateMessageRequest request);
    Task<bool> DeleteAsync(Guid id);
}