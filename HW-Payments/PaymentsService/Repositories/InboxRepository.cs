using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Models;

namespace PaymentsService.Repositories;

public class InboxRepository : IInboxRepository
{
    private readonly PaymentDbContext _dbContext;

    public InboxRepository(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> HasProcessedMessageAsync(string messageId)
    {
        return await _dbContext.InboxMessages
            .AnyAsync(m => m.MessageId == messageId && m.Status == InboxMessageStatus.Processed);
    }

    public async Task<IEnumerable<InboxMessage>> GetPendingInboxMessagesAsync(int batchSize = 10)
    {
        return await _dbContext.InboxMessages
            .Where(m => m.Status == InboxMessageStatus.Pending)
            .OrderBy(m => m.ReceivedAt)
            .Take(batchSize)
            .ToListAsync();
    }

    public async Task MarkInboxMessageAsProcessedAsync(Guid id)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            var message = await _dbContext.InboxMessages.FindAsync(id);
            if (message != null)
            {
                message.Status = InboxMessageStatus.Processed;
                message.ProcessedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
        });
    }

    public async Task MarkInboxMessageAsFailedAsync(Guid id, string error)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            var message = await _dbContext.InboxMessages.FindAsync(id);
            if (message != null)
            {
                message.Status = InboxMessageStatus.Failed;
                message.Error = error;
                await _dbContext.SaveChangesAsync();
            }
        });
    }

    public async Task<Guid> CreateInboxMessageAsync(InboxMessage message)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            if (message.Id == Guid.Empty)
            {
                message.Id = Guid.NewGuid();
            }

            message.ReceivedAt = DateTime.UtcNow;
            message.Status = InboxMessageStatus.Pending;

            _dbContext.InboxMessages.Add(message);
            await _dbContext.SaveChangesAsync();

            return message.Id;
        });
    }
}