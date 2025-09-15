using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.Interfaces.Factories;

public interface IOperationFactory
{
    public Operation Create(OperationType type, Guid bankAccountId, decimal amount, DateTime date, Guid categoryId,
        string description = null);

    public Operation CreateWithId(Guid id, OperationType type, Guid bankAccountId, decimal amount, DateTime date,
        Guid categoryId, string description = null);
}