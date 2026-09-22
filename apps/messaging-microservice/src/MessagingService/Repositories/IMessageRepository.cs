using MessagingService.Domain.Entities;

namespace MessagingService.Repositories;

public interface IMessageRepository
{
    Task<Message?> GetByIdAsync(Guid id);
    Task<List<Message>> GetByChannelIdAsync(Guid channelId);
    Task AddAsync(Message message);
    Task UpdateAsync(Message message);
    Task DeleteAsync(Guid id);
}