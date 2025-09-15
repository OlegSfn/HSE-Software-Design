namespace Shared.Messaging.Messages;

public class PaymentStatusMessage
{
    public string PaymentId { get; set; }
    public Guid OrderId { get; set; }
    public string UserId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public string ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; }

    public bool Success => Status == "Completed";
    public string Reason => ErrorMessage;
}