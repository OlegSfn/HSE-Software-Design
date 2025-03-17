using HSEBank.FinancialAccounting.Models;

namespace HSEBank.FinancialAccounting.Interfaces.Repositories;

public interface IBankAccountRepository : IRepository<BankAccount>
{
    BankAccount GetByName(string name);
}