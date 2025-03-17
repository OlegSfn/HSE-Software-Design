using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Models;

namespace HSEBank.FinancialAccounting.Proxies;

public class CachedBankAccountRepository : IBankAccountRepository
{
    private readonly IBankAccountRepository _repository;
    private readonly Dictionary<Guid, BankAccount> _cache = new();
    private bool _isCacheInitialized;
    
    public CachedBankAccountRepository(IBankAccountRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    private void InitializeCache()
    {
        if (_isCacheInitialized)
        {
            return;
        }
        
        foreach (var bankAccount in _repository.GetAll())
        {
            _cache[bankAccount.Id] = bankAccount;
        }
        
        _isCacheInitialized = true;
    }

    public IEnumerable<BankAccount> GetAll()
    {
        InitializeCache();
        return _cache.Values;
    }
    
    public BankAccount GetById(Guid id)
    {
        InitializeCache();
        return _cache.TryGetValue(id, out var bankAccount) ? bankAccount : null;
    }
    
    public BankAccount GetByName(string name)
    {
        InitializeCache();
        return _cache.Values.FirstOrDefault(ba => ba.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
    
    public void Add(BankAccount bankAccount)
    {
        if (bankAccount == null)
        {
            throw new ArgumentNullException(nameof(bankAccount));
        }

        _repository.Add(bankAccount);
            
        InitializeCache();
        _cache[bankAccount.Id] = bankAccount;
    }
    
    public void Update(BankAccount bankAccount)
    {
        if (bankAccount == null)
        {
            throw new ArgumentNullException(nameof(bankAccount));
        }

        _repository.Update(bankAccount);
            
        InitializeCache();
        _cache[bankAccount.Id] = bankAccount;
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