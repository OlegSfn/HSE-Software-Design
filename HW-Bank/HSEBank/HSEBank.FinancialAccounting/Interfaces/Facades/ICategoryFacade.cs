using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.Interfaces.Facades;

public interface ICategoryFacade
{
    IEnumerable<Category> GetAllCategories();
    Category GetCategoryById(Guid id);
    Category GetCategoryByName(string name);
    IEnumerable<Category> GetCategoriesByType(CategoryType type);
    Category CreateCategory(CategoryType type, string name);
    Category UpdateCategoryName(Guid id, string name);
    Category UpdateCategoryType(Guid id, CategoryType type);
    bool DeleteCategory(Guid id);
}