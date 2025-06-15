using PaymentsService.Models;

namespace PaymentsService.Repositories;

public interface ITransactionRepository
{
    Task<Transaction> GetByIdAsync(Guid id);
    Task<IEnumerable<Transaction>> GetByAccountIdAsync(Guid accountId);
    Task<Transaction> CreateTransactionAsync(Transaction transaction);
    Task<bool> UpdateTransactionStatusAsync(Guid id, TransactionStatus status, string error = null);
    Task<bool> ExistsWithExternalIdAsync(string externalId);
}