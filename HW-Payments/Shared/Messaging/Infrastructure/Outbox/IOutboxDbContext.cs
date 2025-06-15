using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Shared.Messaging.Infrastructure.Outbox;

public interface IOutboxDbContext
{
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}