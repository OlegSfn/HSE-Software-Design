using Microsoft.AspNetCore.Mvc;
using OrdersService.DTOs;
using OrdersService.Models;
using OrdersService.Services;

namespace OrdersService.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> _)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
    {
        if (!Request.Headers.TryGetValue("X-User-Id", out var userIdValue) || string.IsNullOrEmpty(userIdValue))
        {
            return Unauthorized("User ID is missing from request headers");
        }

        string userId = userIdValue.ToString();
        var orders = await _orderService.GetOrdersByUserIdAsync(userId);

        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Order>> GetOrder(Guid id)
    {
        if (!Request.Headers.TryGetValue("X-User-Id", out var userIdValue) || string.IsNullOrEmpty(userIdValue))
        {
            return Unauthorized("User ID is missing from request headers");
        }

        string userId = userIdValue.ToString();
        var order = await _orderService.GetOrderByIdAsync(id, userId);

        if (order == null)
        {
            return NotFound($"Order with ID {id} not found");
        }

        return Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        if (!Request.Headers.TryGetValue("X-User-Id", out var userIdValue) || string.IsNullOrEmpty(userIdValue))
        {
            return Unauthorized("User ID is missing from request headers");
        }

        string userId = userIdValue.ToString();

        if (request.Price <= 0)
        {
            return BadRequest("Order price must be greater than zero");
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            return BadRequest("Order description is required");
        }

        var orderId = await _orderService.CreateOrderAsync(userId, request.Price, request.Description);

        return CreatedAtAction(nameof(GetOrder), new { id = orderId }, orderId);
    }
}