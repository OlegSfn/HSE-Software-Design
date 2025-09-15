namespace Shared.Messaging.Infrastructure.MessageBroker;

public interface IMessageBroker
{
    Task PublishAsync<T>(string queue, T message);
    Task SubscribeAsync<T>(string queue, Func<T, Task> handler);
}