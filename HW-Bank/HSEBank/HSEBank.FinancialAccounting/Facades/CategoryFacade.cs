using HSEBank.FinancialAccounting.Interfaces.Facades;
using HSEBank.FinancialAccounting.Interfaces.Factories;
using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.Facades;

public class CategoryFacade : ICategoryFacade
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICategoryFactory _categoryFactory;

    public CategoryFacade(ICategoryRepository categoryRepository, ICategoryFactory categoryFactory)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _categoryFactory = categoryFactory ?? throw new ArgumentNullException(nameof(categoryFactory));
    }

    public IEnumerable<Category> GetAllCategories() => _categoryRepository.GetAll();

    public Category GetCategoryById(Guid id) => _categoryRepository.GetById(id);

    public Category GetCategoryByName(string name) => _categoryRepository.GetByName(name);

    public IEnumerable<Category> GetCategoriesByType(CategoryType type) => _categoryRepository.GetByType(type);

    public Category CreateCategory(CategoryType type, string name)
    {
        var category = _categoryFactory.Create(type, name);
        _categoryRepository.Add(category);
            
        return category;
    }

    public Category UpdateCategoryName(Guid id, string name)
    {
        var category = _categoryRepository.GetById(id);
        if (category == null)
        {
            throw new ArgumentException($"Category with ID {id} not found");
        }

        category.UpdateName(name);
        _categoryRepository.Update(category);
            
        return category;
    }

    public Category UpdateCategoryType(Guid id, CategoryType type)
    {
        var category = _categoryRepository.GetById(id);
        if (category == null)
        {
            throw new ArgumentException($"Category with ID {id} not found");
        }

        category.UpdateType(type);
        _categoryRepository.Update(category);
            
        return category;
    }

    public bool DeleteCategory(Guid id) => _categoryRepository.Delete(id);
}