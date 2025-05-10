using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.Proxies;

public class CachedOperationRepository : IOperationRepository
{
    private readonly IOperationRepository _repository;
    private readonly Dictionary<Guid, Operation> _cache = new();
    private bool _isCacheInitialized;

    public CachedOperationRepository(IOperationRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    private void InitializeCache()
    {
        if (!_isCacheInitialized)
        {
            foreach (var operation in _repository.GetAll())
            {
                _cache[operation.Id] = operation;
            }
            _isCacheInitialized = true;
        }
    }

    public IEnumerable<Operation> GetAll()
    {
        InitializeCache();
        
        return _cache.Values;
    }

    public Operation GetById(Guid id)
    {
        InitializeCache();
        
        return _cache.TryGetValue(id, out var operation) ? operation : null;
    }

    public IEnumerable<Operation> GetByBankAccount(Guid bankAccountId)
    {
        InitializeCache();
        
        return _cache.Values.Where(o => o.BankAccountId == bankAccountId);
    }

    public IEnumerable<Operation> GetByCategory(Guid categoryId)
    {
        InitializeCache();
        
        return _cache.Values.Where(o => o.CategoryId == categoryId);
    }

    public IEnumerable<Operation> GetByType(OperationType type)
    {
        InitializeCache();
        
        return _cache.Values.Where(o => o.Type == type);
    }

    public IEnumerable<Operation> GetByDateRange(DateTime startDate, DateTime endDate)
    {
        InitializeCache();
        
        return _cache.Values.Where(o => o.Date >= startDate && o.Date <= endDate);
    }

    public IEnumerable<Operation> GetByBankAccountAndDateRange(Guid bankAccountId, DateTime startDate, DateTime endDate)
    {
        InitializeCache();
        
        return _cache.Values.Where(o => o.BankAccountId == bankAccountId && o.Date >= startDate && o.Date <= endDate);
    }

    public void Add(Operation operation)
    {
        if (operation == null)
        {
            throw new ArgumentNullException(nameof(operation));
        }

        _repository.Add(operation);
            
        InitializeCache();
        _cache[operation.Id] = operation;
    }

    public void Update(Operation operation)
    {
        if (operation == null)
        {
            throw new ArgumentNullException(nameof(operation));
        }

        _repository.Update(operation);
            
        InitializeCache();
        _cache[operation.Id] = operation;
    }

    public bool Delete(Guid id)
    {
        var result = _repository.Delete(id);

        if (!result)
        {
            return result;
        }
        
        InitializeCache();
        _cache.Remove(id);

        return result;
    }
}