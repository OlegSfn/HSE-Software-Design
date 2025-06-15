namespace OrdersService.DTOs;

public class CreateOrderRequest
{
    public decimal Price { get; set; }
    public string Description { get; set; }
}