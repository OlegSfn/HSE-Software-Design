using System.ComponentModel.DataAnnotations;

namespace OrdersService.Models;

public class Order
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public OrderStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

public enum OrderStatus
{
    NEW,
    FINISHED,
    CANCELLED
}