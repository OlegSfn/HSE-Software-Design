using HSEBank.FinancialAccounting.Models;

namespace HSEBank.FinancialAccounting.Interfaces.Factories;

public interface IBankAccountFactory
{
    BankAccount Create(string name, decimal initialBalance);
    BankAccount CreateWithId(Guid id, string name, decimal balance);
}