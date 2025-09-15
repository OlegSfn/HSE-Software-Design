using System.ComponentModel.DataAnnotations;

namespace Shared.Messaging.Infrastructure.Outbox;

public class OutboxMessage
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Type { get; set; }

    [Required]
    public string Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public OutboxMessageStatus Status { get; set; }

    public string? Error { get; set; }
}

public enum OutboxMessageStatus
{
    Pending,
    Processed,
    Failed
}