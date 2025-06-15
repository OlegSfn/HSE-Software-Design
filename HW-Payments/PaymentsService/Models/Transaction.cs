using System.ComponentModel.DataAnnotations;

namespace PaymentsService.Models;

public class Transaction
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid AccountId { get; set; }

    [Required]
    public string UserId { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public TransactionType Type { get; set; }

    [Required]
    public TransactionStatus Status { get; set; }

    public string Description { get; set; }

    public Guid? OrderId { get; set; }

    public string ExternalId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }
}

public enum TransactionType
{
    Deposit,
    Payment
}

public enum TransactionStatus
{
    Pending,
    Completed,
    Failed
}