using HSEBank.FinancialAccounting.Models;

namespace HSEBank.FinancialAccounting.Interfaces.Facades;

public interface IBankAccountFacade
{
    IEnumerable<BankAccount> GetAllBankAccounts();
    BankAccount GetBankAccountById(Guid id);
    BankAccount GetBankAccountByName(string name);
    BankAccount CreateBankAccount(string name, decimal initialBalance);
    BankAccount UpdateBankAccountName(Guid id, string name);
    BankAccount UpdateBankAccountBalance(Guid id, decimal balance);
    bool DeleteBankAccount(Guid id);
}