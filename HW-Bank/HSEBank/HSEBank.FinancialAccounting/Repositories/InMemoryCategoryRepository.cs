using HSEBank.FinancialAccounting.Interfaces;
using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.Repositories;

public class InMemoryCategoryRepository : ICategoryRepository
{
    private readonly Dictionary<Guid, Category> _categories = new();

    public IEnumerable<Category> GetAll() => _categories.Values;

    public Category GetById(Guid id) 
        => _categories.TryGetValue(id, out var category) ? category : null;

    public Category GetByName(string name)
        => _categories.Values.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Category> GetByType(CategoryType type)
        => _categories.Values.Where(c => c.Type == type);

    public void Add(Category category)
    {
        if (category == null)
        {
            throw new ArgumentNullException(nameof(category));
        }

        if (_categories.ContainsKey(category.Id))
        {
            throw new ArgumentException($"Category with ID {category.Id} already exists");
        }

        _categories[category.Id] = category;
    }

    public void Update(Category category)
    {
        if (category == null)
        {
            throw new ArgumentNullException(nameof(category));
        }

        if (!_categories.ContainsKey(category.Id))
        {
            throw new ArgumentException($"Category with ID {category.Id} does not exist");
        }

        _categories[category.Id] = category;
    }

    public bool Delete(Guid id)
        => _categories.Remove(id);
}