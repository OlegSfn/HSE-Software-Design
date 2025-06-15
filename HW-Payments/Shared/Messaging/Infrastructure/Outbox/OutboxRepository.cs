using Microsoft.EntityFrameworkCore;

namespace Shared.Messaging.Infrastructure.Outbox;

public class OutboxRepository : IOutboxRepository
{
    public readonly IOutboxDbContext DbContext;
    
    public OutboxRepository(IOutboxDbContext dbContext)
    {
        DbContext = dbContext;
    }
    
    public async Task<IEnumerable<OutboxMessage>> GetPendingOutboxMessagesAsync(int batchSize = 10)
    {
        return await DbContext.OutboxMessages
            .Where(m => m.Status == OutboxMessageStatus.Pending)
            .OrderBy(m => m.CreatedAt)
            .Take(batchSize)
            .ToListAsync();
    }

    public async Task MarkOutboxMessageAsProcessedAsync(Guid id)
    {
        var strategy = DbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            var message = await DbContext.OutboxMessages.FindAsync(id);
            if (message != null)
            {
                message.Status = OutboxMessageStatus.Processed;
                message.ProcessedAt = DateTime.UtcNow;
                await DbContext.SaveChangesAsync();
            }
        });
    }

    public async Task MarkOutboxMessageAsFailedAsync(Guid id, string error)
    {
        var strategy = DbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            var message = await DbContext.OutboxMessages.FindAsync(id);
            if (message != null)
            {
                message.Status = OutboxMessageStatus.Failed;
                message.Error = error;
                await DbContext.SaveChangesAsync();
            }
        });
    }

    public async Task<Guid> CreateOutboxMessageAsync(OutboxMessage message)
    {
        var strategy = DbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            message.Id = Guid.NewGuid();
            message.CreatedAt = DateTime.UtcNow;
            message.Status = OutboxMessageStatus.Pending;

            DbContext.OutboxMessages.Add(message);
            await DbContext.SaveChangesAsync();

            return message.Id;
        });
    }
}