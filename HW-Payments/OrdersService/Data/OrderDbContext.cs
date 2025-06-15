using Microsoft.EntityFrameworkCore;
using OrdersService.Models;
using Shared.Messaging.Infrastructure;
using Shared.Messaging.Infrastructure.Outbox;
using Shared.Messaging.Messages;

namespace OrdersService.Data;

public class OrderDbContext : DbContext, IOutboxDbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>()
            .Property(o => o.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Order>()
            .HasIndex(o => o.UserId);
    }
}