using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.Interfaces.Facades;

public interface IOperationFacade
{
    IEnumerable<Operation> GetAllOperations();
    IEnumerable<Operation> GetOperationsByBankAccount(Guid bankAccountId);
    IEnumerable<Operation> GetOperationsByDateRange(DateTime startDate, DateTime endDate);
    Operation GetOperationById(Guid id);
    Operation CreateOperation(OperationType type, Guid bankAccountId, decimal amount, DateTime date, Guid categoryId, string description = null);
    bool DeleteOperation(Guid id);
}