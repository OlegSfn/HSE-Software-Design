using HSEBank.FinancialAccounting.Interfaces.Factories;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.Factories;

public class OperationFactory : IOperationFactory
{
    public Operation Create(OperationType type, Guid bankAccountId, decimal amount, DateTime date, Guid categoryId, string description = null)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(amount));
        }

        return new Operation(Guid.NewGuid(), type, bankAccountId, amount, date, categoryId, description);
    }
    
    public Operation CreateWithId(Guid id, OperationType type, Guid bankAccountId, decimal amount, DateTime date, Guid categoryId, string description = null)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(amount));
        }

        return new Operation(id, type, bankAccountId, amount, date, categoryId, description);
    }
}