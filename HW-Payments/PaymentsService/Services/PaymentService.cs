using System.Text.Json;
using PaymentsService.Models;
using PaymentsService.Repositories;
using Shared.Messaging.Infrastructure;
using Shared.Messaging.Infrastructure.Outbox;
using Shared.Messaging.Messages;

namespace PaymentsService.Services;

public class PaymentService : IPaymentService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IAccountRepository accountRepository, ITransactionRepository transactionRepository, IOutboxRepository outboxRepository, ILogger<PaymentService> logger)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
        _outboxRepository = outboxRepository;
        _logger = logger;
    }

    public async Task<(bool success, string error)> ProcessPaymentRequestAsync(PaymentRequestMessage paymentRequest)
    {
        if (await _transactionRepository.ExistsWithExternalIdAsync(paymentRequest.PaymentId))
        {
            return (true, null);
        }

        var account = await _accountRepository.GetByUserIdAsync(paymentRequest.UserId);
        if (account == null)
        {
            return (false, "Account not found");
        }

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            UserId = paymentRequest.UserId,
            Amount = paymentRequest.Amount,
            Type = TransactionType.Payment,
            Status = TransactionStatus.Pending,
            Description = $"Payment for Order {paymentRequest.OrderId}",
            OrderId = paymentRequest.OrderId,
            ExternalId = paymentRequest.PaymentId,
            CreatedAt = DateTime.UtcNow
        };

        await _transactionRepository.CreateTransactionAsync(transaction);

        var (debitSuccess, debitError) = await _accountRepository.TryDebitAccountAsync(account.Id, paymentRequest.Amount);

        var paymentStatus = new PaymentStatusMessage
        {
            PaymentId = paymentRequest.PaymentId,
            OrderId = paymentRequest.OrderId,
            UserId = paymentRequest.UserId,
            Amount = paymentRequest.Amount,
            Status = debitSuccess ? "Completed" : "Failed",
            ErrorMessage = debitError,
            Timestamp = DateTime.UtcNow
        };

        _logger.LogInformation("Created payment status message: OrderId={OrderId}, Status={Status}, Success={Success}",
            paymentStatus.OrderId, paymentStatus.Status, debitSuccess);

        var outboxMessage = new OutboxMessage
        {
            Type = "PaymentStatus",
            Content = JsonSerializer.Serialize(paymentStatus),
            CreatedAt = DateTime.UtcNow,
            Status = OutboxMessageStatus.Pending
        };

        await _outboxRepository.CreateOutboxMessageAsync(outboxMessage);

        await _transactionRepository.UpdateTransactionStatusAsync(
            transaction.Id,
            debitSuccess ? TransactionStatus.Completed : TransactionStatus.Failed,
            debitError);

        return (debitSuccess, debitError);
    }

    public async Task<Transaction> GetPaymentByIdAsync(Guid id)
        => await _transactionRepository.GetByIdAsync(id);

    public async Task<IEnumerable<Transaction>> GetPaymentsByAccountIdAsync(Guid accountId)
    {
        var transactions = await _transactionRepository.GetByAccountIdAsync(accountId);
        return transactions.Where(t => t.Type == TransactionType.Payment);
    }

    public async Task<IEnumerable<Transaction>> GetPaymentsByUserIdAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
        }

        var account = await _accountRepository.GetByUserIdAsync(userId);
        if (account == null)
        {
            return Enumerable.Empty<Transaction>();
        }

        return await GetPaymentsByAccountIdAsync(account.Id);
    }

    public async Task<IEnumerable<Transaction>> GetAllTransactionsByUserIdAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
        }

        _logger.LogInformation("Getting all transactions for user {UserId}", userId);

        var account = await _accountRepository.GetByUserIdAsync(userId);
        if (account == null)
        {
            _logger.LogWarning("Account not found for user {UserId}", userId);
            return Enumerable.Empty<Transaction>();
        }

        var transactions = await _transactionRepository.GetByAccountIdAsync(account.Id);
        _logger.LogInformation("Found {Count} transactions for user {UserId}", transactions.Count(), userId);

        return transactions;
    }
}