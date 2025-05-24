using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.Proxies;

public class CachedCategoryRepository : ICategoryRepository
{
    private readonly ICategoryRepository _repository;
    private readonly Dictionary<Guid, Category> _cache = new();
    private bool _isCacheInitialized;

    public CachedCategoryRepository(ICategoryRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    private void InitializeCache()
    {
        if (_isCacheInitialized)
        {
            return;
        }
        
        foreach (var category in _repository.GetAll())
        {
            _cache[category.Id] = category;
        }
        
        _isCacheInitialized = true;
    }

    public IEnumerable<Category> GetAll()
    {
        InitializeCache();
        
        return _cache.Values;
    }

    public Category GetById(Guid id)
    {
        InitializeCache();
        
        return _cache.TryGetValue(id, out var category) ? category : null;
    }

    public Category GetByName(string name)
    {
        InitializeCache();
        
        return _cache.Values.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Category> GetByType(CategoryType type)
    {
        InitializeCache();
        
        return _cache.Values.Where(c => c.Type == type);
    }

    public void Add(Category category)
    {
        if (category == null)
        {
            throw new ArgumentNullException(nameof(category));
        }

        _repository.Add(category);
            
        InitializeCache();
        _cache[category.Id] = category;
    }

    public void Update(Category category)
    {
        if (category == null)
        {
            throw new ArgumentNullException(nameof(category));
        }

        _repository.Update(category);
            
        InitializeCache();
        _cache[category.Id] = category;
    }

    public bool Delete(Guid id)
    {
        var result = _repository.Delete(id);
            
        if (result)
        {
            InitializeCache();
            _cache.Remove(id);
        }
            
        return result;
    }
}