using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Models;

namespace PaymentsService.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly PaymentDbContext _dbContext;

    public TransactionRepository(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Transaction> GetByIdAsync(Guid id)
        => await _dbContext.Transactions.FindAsync(id);

    public async Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId)
    {
        return await _dbContext.Transactions
            .Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Transaction> CreateTransactionAsync(Transaction transaction)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            if (transaction.CreatedAt == default)
            {
                transaction.CreatedAt = DateTime.UtcNow;
            }

            _dbContext.Transactions.Add(transaction);
            await _dbContext.SaveChangesAsync();
            return transaction;
        });
    }

    public async Task<bool> UpdateTransactionStatusAsync(Guid id, TransactionStatus status, string error = null)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            var transaction = await _dbContext.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return false;
            }

            transaction.Status = status;

            if (status == TransactionStatus.Completed || status == TransactionStatus.Failed)
            {
                transaction.CompletedAt = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();
            return true;
        });
    }

    public async Task<bool> ExistsWithExternalIdAsync(string externalId)
    {
        if (string.IsNullOrEmpty(externalId))
        {
            return false;
        }

        return await _dbContext.Transactions
            .AnyAsync(t => t.ExternalId == externalId);
    }
}