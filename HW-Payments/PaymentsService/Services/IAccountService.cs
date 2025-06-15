using PaymentsService.Models;

namespace PaymentsService.Services;

public interface IAccountService
{
    Task<Account> GetAccountByUserIdAsync(string userId);
    Task<bool> CreateAccountAsync(string userId, decimal initialBalance = 0);
    Task<(bool success, string error)> DepositFundsAsync(string userId, decimal amount, string description);
    Task<decimal> GetBalanceAsync(string userId);
}