using EasyNetQ;

namespace MessagingService.Messaging;

public static class MessageClientExtensions
{
    public static IServiceCollection AddMessageClient(this IServiceCollection services, string connectionString)
    {
        services.AddEasyNetQ(connectionString).UseSystemTextJson();
        services.AddSingleton<IMessageClient, RabbitMqMessageClient>();
        return services;
    }
}