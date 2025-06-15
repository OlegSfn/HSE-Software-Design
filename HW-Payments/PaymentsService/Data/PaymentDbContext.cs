using Microsoft.EntityFrameworkCore;
using PaymentsService.Models;
using Shared.Messaging.Infrastructure;
using Shared.Messaging.Infrastructure.Outbox;
using Shared.Messaging.Messages;

namespace PaymentsService.Data;

public class PaymentDbContext : DbContext, IOutboxDbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<InboxMessage> InboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>()
            .Property(a => a.Balance)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Account>()
            .HasIndex(a => a.UserId)
            .IsUnique();

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.AccountId);

        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.ExternalId)
            .IsUnique();

        modelBuilder.Entity<InboxMessage>()
            .HasIndex(m => m.MessageId)
            .IsUnique();
    }
}