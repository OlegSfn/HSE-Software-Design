namespace Shared.Messaging.Infrastructure.Outbox;

public interface IOutboxRepository
{
    Task<IEnumerable<OutboxMessage>> GetPendingOutboxMessagesAsync(int batchSize = 10);
    Task MarkOutboxMessageAsProcessedAsync(Guid id);
    Task MarkOutboxMessageAsFailedAsync(Guid id, string error);
    Task<Guid> CreateOutboxMessageAsync(OutboxMessage message);
}