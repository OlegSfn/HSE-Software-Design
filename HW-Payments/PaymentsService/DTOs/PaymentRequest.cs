namespace PaymentsService.DTOs;

public class PaymentRequest
{
    public string? PaymentId { get; set; }
    public Guid OrderId { get; set; }
    public string UserId { get; set; }
    public decimal Amount { get; set; }
}