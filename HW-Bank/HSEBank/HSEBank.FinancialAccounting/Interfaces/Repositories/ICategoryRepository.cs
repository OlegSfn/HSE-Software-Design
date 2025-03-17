using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.Interfaces.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Category GetByName(string name);
    IEnumerable<Category> GetByType(CategoryType type);
}