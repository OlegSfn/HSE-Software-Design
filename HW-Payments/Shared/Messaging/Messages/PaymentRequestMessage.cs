namespace Shared.Messaging.Messages;

public class PaymentRequestMessage
{
    public string PaymentId { get; set; }
    public Guid OrderId { get; set; }
    public string UserId { get; set; }
    public decimal Amount { get; set; }
}