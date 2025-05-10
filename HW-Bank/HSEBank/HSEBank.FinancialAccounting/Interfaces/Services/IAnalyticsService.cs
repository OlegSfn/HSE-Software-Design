using HSEBank.FinancialAccounting.Models;

namespace HSEBank.FinancialAccounting.Interfaces.Services;

public interface IAnalyticsService
{
    decimal CalculateIncomeExpenseDifference(Guid bankAccountId, DateTime startDate, DateTime endDate);
    Dictionary<Category, decimal> GroupOperationsByCategory(Guid bankAccountId, DateTime startDate, DateTime endDate);
    decimal CalculateTotalIncome(Guid bankAccountId, DateTime startDate, DateTime endDate);
    decimal CalculateTotalExpenses(Guid bankAccountId, DateTime startDate, DateTime endDate);
}