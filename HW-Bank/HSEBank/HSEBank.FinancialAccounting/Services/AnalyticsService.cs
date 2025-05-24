using HSEBank.FinancialAccounting.Interfaces;
using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Interfaces.Services;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IOperationRepository _operationRepository;
    private readonly ICategoryRepository _categoryRepository;

    public AnalyticsService(IOperationRepository operationRepository, ICategoryRepository categoryRepository)
    {
        _operationRepository = operationRepository ?? throw new ArgumentNullException(nameof(operationRepository));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public decimal CalculateIncomeExpenseDifference(Guid bankAccountId, DateTime startDate, DateTime endDate)
    {
        var totalIncome = CalculateTotalIncome(bankAccountId, startDate, endDate);
        var totalExpenses = CalculateTotalExpenses(bankAccountId, startDate, endDate);
        
        return totalIncome - totalExpenses;
    }

    public Dictionary<Category, decimal> GroupOperationsByCategory(Guid bankAccountId, DateTime startDate, DateTime endDate)
    {
        var operations = _operationRepository.GetByBankAccountAndDateRange(bankAccountId, startDate, endDate);
        var result = new Dictionary<Category, decimal>();

        foreach (var operation in operations)
        {
            var category = _categoryRepository.GetById(operation.CategoryId);
            if (category == null)
            {
                continue;
            }

            if (result.ContainsKey(category))
            {
                result[category] += operation.Amount;
            }
            else
            {
                result[category] = operation.Amount;
            }
        }

        return result;
    }
    
    public decimal CalculateTotalIncome(Guid bankAccountId, DateTime startDate, DateTime endDate)
    {
        var operations = _operationRepository.GetByBankAccountAndDateRange(bankAccountId, startDate, endDate);
        
        return operations.Where(o => o.Type == OperationType.Income).Sum(o => o.Amount);
    }

    public decimal CalculateTotalExpenses(Guid bankAccountId, DateTime startDate, DateTime endDate)
    {
        var operations = _operationRepository.GetByBankAccountAndDateRange(bankAccountId, startDate, endDate);
        
        return operations.Where(o => o.Type == OperationType.Expense).Sum(o => o.Amount);
    }
}