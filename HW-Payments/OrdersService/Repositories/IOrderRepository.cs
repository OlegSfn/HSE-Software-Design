using OrdersService.Models;
using Shared.Messaging.Infrastructure.Outbox;
using Shared.Messaging.Messages;

namespace OrdersService.Repositories;

public interface IOrderRepository
{
    Task<Order> GetOrderByIdAsync(Guid id);
    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
    Task<Guid> CreateOrderAsync(Order order, OutboxMessage outboxMessage);
    Task UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
}