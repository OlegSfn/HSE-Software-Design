using Microsoft.EntityFrameworkCore;
using PaymentsService.Data;
using PaymentsService.Models;

namespace PaymentsService.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly PaymentDbContext _dbContext;

    public AccountRepository(PaymentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Account> GetByUserIdAsync(string userId)
        => await _dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);

    public async Task<bool> CreateAccountAsync(Account account)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            if (await _dbContext.Accounts.AnyAsync(a => a.UserId == account.UserId))
            {
                return false;
            }

            account.CreatedAt = DateTime.UtcNow;

            _dbContext.Accounts.Add(account);
            await _dbContext.SaveChangesAsync();
            return true;
        });
    }

    public async Task<bool> UpdateBalanceAsync(Guid accountId, decimal amount)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            var account = await _dbContext.Accounts.FindAsync(accountId);
            if (account == null)
            {
                return false;
            }

            account.Balance += amount;
            account.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        });
    }

    public async Task<(bool success, string error)> TryDebitAccountAsync(Guid accountId, decimal amount)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            var account = await _dbContext.Accounts.FindAsync(accountId);
            if (account == null)
            {
                return (false, "Account not found");
            }

            if (account.Balance < amount)
            {
                return (false, "Insufficient funds");
            }

            account.Balance -= amount;
            account.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _dbContext.SaveChangesAsync();
                return (true, null);
            }
            catch (DbUpdateConcurrencyException)
            {
                return (false, "Concurrent update detected, please try again");
            }
        });
    }
}