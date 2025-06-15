using PaymentsService.Models;
using PaymentsService.Repositories;
using Transaction = PaymentsService.Models.Transaction;

namespace PaymentsService.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly ITransactionRepository _transactionRepository;

    public AccountService(IAccountRepository accountRepository, ITransactionRepository transactionRepository)
    {
        _accountRepository = accountRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<Account> GetAccountByUserIdAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
        }

        var account = await _accountRepository.GetByUserIdAsync(userId);
        return account;
    }

    public async Task<bool> CreateAccountAsync(string userId, decimal initialBalance = 0)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
        }

        if (initialBalance < 0)
        {
            throw new ArgumentException("Initial balance cannot be negative", nameof(initialBalance));
        }

        var existingAccount = await _accountRepository.GetByUserIdAsync(userId);
        if (existingAccount != null)
        {
            return false;
        }

        var account = new Account
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Balance = initialBalance,
            CreatedAt = DateTime.UtcNow
        };

        return await _accountRepository.CreateAccountAsync(account);
    }

    public async Task<(bool success, string error)> DepositFundsAsync(string userId, decimal amount, string description)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return (false, "User ID cannot be null or empty");
        }

        if (amount <= 0)
        {
            return (false, "Deposit amount must be greater than zero");
        }

        var account = await _accountRepository.GetByUserIdAsync(userId);
        if (account == null)
        {
            return (false, "Account not found");
        }

        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            UserId = userId,
            Amount = amount,
            Type = TransactionType.Deposit,
            Status = TransactionStatus.Pending,
            Description = description ?? "Deposit",
            CreatedAt = DateTime.UtcNow,
            ExternalId = Guid.NewGuid().ToString()
        };

        var createdTransaction = await _transactionRepository.CreateTransactionAsync(transaction);

        var updated = await _accountRepository.UpdateBalanceAsync(account.Id, amount);
        if (!updated)
        {
            await _transactionRepository.UpdateTransactionStatusAsync(
                createdTransaction.Id,
                TransactionStatus.Failed,
                "Failed to update account balance");

            return (false, "Failed to update account balance due to a concurrency issue. Please try again.");
        }

        await _transactionRepository.UpdateTransactionStatusAsync(
            createdTransaction.Id,
            TransactionStatus.Completed);

        return (true, null);
    }

    public async Task<decimal> GetBalanceAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
        }

        var account = await _accountRepository.GetByUserIdAsync(userId);
        if (account == null)
        {
            throw new InvalidOperationException($"Account not found for user ID: {userId}");
        }

        return account.Balance;
    }
}