using System.ComponentModel.DataAnnotations;

namespace PaymentsService.Models;

public class InboxMessage
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string MessageId { get; set; }

    [Required]
    public string Type { get; set; }

    [Required]
    public string Content { get; set; }

    public DateTime ReceivedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public InboxMessageStatus Status { get; set; }

    public string? Error { get; set; }
}

public enum InboxMessageStatus
{
    Pending,
    Processed,
    Failed
}