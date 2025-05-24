using HSEBank.FinancialAccounting.Models;

namespace HSEBank.FinancialAccounting.Interfaces.Facades;

public interface IAnalyticsFacade
{
    decimal CalculateTotalIncome(DateTime startDate, DateTime endDate);
    decimal CalculateTotalExpenses(DateTime startDate, DateTime endDate);
    decimal CalculateTotalIncome(Guid bankAccountId, DateTime startDate, DateTime endDate);
    decimal CalculateTotalExpenses(Guid bankAccountId, DateTime startDate, DateTime endDate);
    decimal CalculateIncomeExpenseDifference(DateTime startDate, DateTime endDate);
    decimal CalculateIncomeExpenseDifference(Guid bankAccountId, DateTime startDate, DateTime endDate);
    Dictionary<Category, decimal> GroupOperationsByCategory(DateTime startDate, DateTime endDate);
    Dictionary<Category, decimal> GroupOperationsByCategory(Guid bankAccountId, DateTime startDate, DateTime endDate);
}