using PaymentsService.Models;

namespace PaymentsService.Repositories;

public interface IInboxRepository
{
    Task<bool> HasProcessedMessageAsync(string messageId);
    Task<IEnumerable<InboxMessage>> GetPendingInboxMessagesAsync(int batchSize = 10);
    Task MarkInboxMessageAsProcessedAsync(Guid id);
    Task MarkInboxMessageAsFailedAsync(Guid id, string error);
    Task<Guid> CreateInboxMessageAsync(InboxMessage message);
}