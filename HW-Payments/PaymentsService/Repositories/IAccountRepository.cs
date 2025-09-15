using PaymentsService.Models;

namespace PaymentsService.Repositories;

public interface IAccountRepository
{
    Task<Account> GetByUserIdAsync(string userId);
    Task<bool> CreateAccountAsync(Account account);
    Task<bool> UpdateBalanceAsync(Guid accountId, decimal amount);
    Task<(bool success, string error)> TryDebitAccountAsync(Guid accountId, decimal amount);
}