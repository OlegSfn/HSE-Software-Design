using HSEBank.FinancialAccounting.Interfaces;
using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.Repositories;

public class InMemoryOperationRepository : IOperationRepository
{
    private readonly Dictionary<Guid, Operation> _operations = new();
    
    public IEnumerable<Operation> GetAll()
        => _operations.Values;

    public Operation GetById(Guid id) 
        => _operations.TryGetValue(id, out var operation) ? operation : null;

    public IEnumerable<Operation> GetByBankAccount(Guid bankAccountId)
        => _operations.Values.Where(o => o.BankAccountId == bankAccountId);

    public IEnumerable<Operation> GetByCategory(Guid categoryId)
        => _operations.Values.Where(o => o.CategoryId == categoryId);

    public IEnumerable<Operation> GetByType(OperationType type)
        => _operations.Values.Where(o => o.Type == type);

    public IEnumerable<Operation> GetByDateRange(DateTime startDate, DateTime endDate)
        => _operations.Values.Where(o => o.Date >= startDate && o.Date <= endDate);

    public IEnumerable<Operation> GetByBankAccountAndDateRange(Guid bankAccountId, DateTime startDate, DateTime endDate)
        => _operations.Values.Where(o => o.BankAccountId == bankAccountId && o.Date >= startDate && o.Date <= endDate);
    
    public void Add(Operation operation)
    {
        if (operation == null)
        {
            throw new ArgumentNullException(nameof(operation));
        }

        if (_operations.ContainsKey(operation.Id))
        {
            throw new ArgumentException($"Operation with ID {operation.Id} already exists");
        }

        _operations[operation.Id] = operation;
    }

    public void Update(Operation operation)
    {
        if (operation == null)
        {
            throw new ArgumentNullException(nameof(operation));
        }

        if (!_operations.ContainsKey(operation.Id))
        {
            throw new ArgumentException($"Operation with ID {operation.Id} does not exist");
        }

        _operations[operation.Id] = operation;
    }

    public bool Delete(Guid id)
        => _operations.Remove(id);
}