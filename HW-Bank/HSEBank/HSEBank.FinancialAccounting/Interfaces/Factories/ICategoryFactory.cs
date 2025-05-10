using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.Interfaces.Factories;

public interface ICategoryFactory
{
    public Category Create(CategoryType type, string name);
    public Category CreateWithId(Guid id, CategoryType type, string name);
}