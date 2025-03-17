using HSEBank.FinancialAccounting.Interfaces.Factories;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.Factories;

public class CategoryFactory : ICategoryFactory
{
    public Category Create(CategoryType type, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name cannot be empty", nameof(name));
        }

        return new Category(Guid.NewGuid(), type, name);
    }

    public Category CreateWithId(Guid id, CategoryType type, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name cannot be empty", nameof(name));
        }

        return new Category(id, type, name);
    }
}