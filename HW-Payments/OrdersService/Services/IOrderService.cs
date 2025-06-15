using OrdersService.Models;

namespace OrdersService.Services;

public interface IOrderService
{
    Task<Order> GetOrderByIdAsync(Guid id, string userId);
    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
    Task<Guid> CreateOrderAsync(string userId, decimal price, string description);
    Task HandlePaymentStatusAsync(Guid orderId, bool success, string reason);
}