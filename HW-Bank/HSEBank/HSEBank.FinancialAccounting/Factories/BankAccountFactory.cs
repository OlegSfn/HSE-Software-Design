using HSEBank.FinancialAccounting.Interfaces.Factories;
using HSEBank.FinancialAccounting.Models;

namespace HSEBank.FinancialAccounting.Factories;

public class BankAccountFactory : IBankAccountFactory
{
    public BankAccount Create(string name, decimal initialBalance)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Account name cannot be empty", nameof(name));
        }

        if (initialBalance < 0)
        {
            throw new ArgumentException("Initial balance cannot be negative", nameof(initialBalance));
        }

        return new BankAccount(Guid.NewGuid(), name, initialBalance);
    }
    
    public BankAccount CreateWithId(Guid id, string name, decimal balance)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Account name cannot be empty", nameof(name));
        }

        if (balance < 0)
        {
            throw new ArgumentException("Balance cannot be negative", nameof(balance));
        }

        return new BankAccount(id, name, balance);
    }
}