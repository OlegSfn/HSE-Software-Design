using Microsoft.EntityFrameworkCore;
using OrdersService.Data;
using OrdersService.Models;
using Shared.Messaging.Infrastructure.Outbox;
using Shared.Messaging.Messages;

namespace OrdersService.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderDbContext _dbContext;

    public OrderRepository(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Order> GetOrderByIdAsync(Guid id)
    {
        return await _dbContext.Orders.FindAsync(id);
    }

    public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
    {
        return await _dbContext.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Guid> CreateOrderAsync(Order order, OutboxMessage outboxMessage)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                order.CreatedAt = DateTime.UtcNow;
                order.Status = OrderStatus.NEW;

                await _dbContext.Orders.AddAsync(order);
                await _dbContext.OutboxMessages.AddAsync(outboxMessage);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return order.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        });
    }

    public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status)
    {
        var order = await GetOrderByIdAsync(orderId);
        if (order != null)
        {
            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }
    }
}