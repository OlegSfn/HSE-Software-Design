using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.Interfaces.Repositories;

public interface IOperationRepository : IRepository<Operation>
{
    IEnumerable<Operation> GetByBankAccount(Guid bankAccountId);
    IEnumerable<Operation> GetByCategory(Guid categoryId);
    IEnumerable<Operation> GetByType(OperationType type);
    IEnumerable<Operation> GetByDateRange(DateTime startDate, DateTime endDate);
    IEnumerable<Operation> GetByBankAccountAndDateRange(Guid bankAccountId, DateTime startDate, DateTime endDate);
}