using Newtonsoft.Json;
using OrdersService.Models;
using OrdersService.Repositories;
using Shared.Messaging.Infrastructure.Outbox;
using Shared.Messaging.Messages;

namespace OrdersService.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IOrderRepository orderRepository, ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<Order> GetOrderByIdAsync(Guid id, string userId)
    {
        var order = await _orderRepository.GetOrderByIdAsync(id);

        if (order != null && order.UserId == userId)
        {
            return order;
        }

        return null;
    }

    public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
    {
        return await _orderRepository.GetOrdersByUserIdAsync(userId);
    }

    public async Task<Guid> CreateOrderAsync(string userId, decimal price, string description)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Price = price,
            Description = description,
            Status = OrderStatus.NEW,
            CreatedAt = DateTime.UtcNow
        };

        var paymentRequest = new PaymentRequestMessage
        {
            PaymentId = Guid.NewGuid().ToString(),
            OrderId = order.Id,
            UserId = userId,
            Amount = price
        };

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = "PaymentRequest",
            Content = JsonConvert.SerializeObject(paymentRequest),
            CreatedAt = DateTime.UtcNow,
            Status = OutboxMessageStatus.Pending
        };

        var orderId = await _orderRepository.CreateOrderAsync(order, outboxMessage);

        _logger.LogInformation("Created order {OrderId} for user {UserId} with price {Price}", orderId, userId, price);

        return orderId;
    }

    public async Task HandlePaymentStatusAsync(Guid orderId, bool success, string reason)
    {
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            _logger.LogWarning("Cannot update order {OrderId} status - order not found", orderId);
            return;
        }

        _logger.LogInformation("Order {OrderId} current status: {CurrentStatus}", orderId, order.Status);

        OrderStatus newStatus = success ? OrderStatus.FINISHED : OrderStatus.CANCELLED;

        if (order.Status != newStatus)
        {
            await _orderRepository.UpdateOrderStatusAsync(orderId, newStatus);
            _logger.LogInformation("Updated order {OrderId} status from {OldStatus} to {NewStatus}. Success: {Success}, Reason: {Reason}",
                orderId, order.Status, newStatus, success, reason);
        }
        else
        {
            _logger.LogInformation("Order {OrderId} status already set to {Status}. No update needed.",
                orderId, order.Status);
        }
    }
}