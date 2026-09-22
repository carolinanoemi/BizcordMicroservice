using MessagingService.Domain.Entities;

namespace MessagingService.Repositories;

public class InMemoryMessageRepository : IMessageRepository
{
    private readonly List<Message> _messages = new();

    public Task<Message?> GetByIdAsync(Guid id)
    {
        var message = _messages.FirstOrDefault(m => m.Id == id && !m.IsDeleted);
        return Task.FromResult(message);
    }

    public Task<List<Message>> GetByChannelIdAsync(Guid channelId)
    {
        var messages = _messages
            .Where(m => m.ChannelId == channelId && !m.IsDeleted)
            .OrderBy(m => m.CreatedAt)
            .ToList();
        return Task.FromResult(messages);
    }

    public Task AddAsync(Message message)
    {
        _messages.Add(message);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Message message)
    {
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        var message = _messages.FirstOrDefault(m => m.Id == id);
        if (message != null)
            message.IsDeleted = true;
        return Task.CompletedTask;
    }
}