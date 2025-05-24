using HSEBank.FinancialAccounting.Interfaces.Facades;
using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Interfaces.Services;
using HSEBank.FinancialAccounting.Models;

namespace HSEBank.FinancialAccounting.Facades;

public class AnalyticsFacade : IAnalyticsFacade
{
    private readonly IAnalyticsService _analyticsService;
    private readonly IBankAccountRepository _bankAccountRepository;

    public AnalyticsFacade(IAnalyticsService analyticsService, IBankAccountRepository bankAccountRepository)
    {
        _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
        _bankAccountRepository = bankAccountRepository ?? throw new ArgumentNullException(nameof(bankAccountRepository));
    }

    public decimal CalculateTotalIncome(DateTime startDate, DateTime endDate)
    {
        var bankAccounts = _bankAccountRepository.GetAll();

        return bankAccounts.Sum(bankAccount => _analyticsService.CalculateTotalIncome(bankAccount.Id, startDate, endDate));
    }

    public decimal CalculateTotalExpenses(DateTime startDate, DateTime endDate)
    {
        var bankAccounts = _bankAccountRepository.GetAll();

        return bankAccounts.Sum(bankAccount => _analyticsService.CalculateTotalExpenses(bankAccount.Id, startDate, endDate));
    }

    public decimal CalculateIncomeExpenseDifference(DateTime startDate, DateTime endDate)
    {
        return CalculateTotalIncome(startDate, endDate) - CalculateTotalExpenses(startDate, endDate);
    }

    public Dictionary<Category, decimal> GroupOperationsByCategory(DateTime startDate, DateTime endDate)
    {
        Dictionary<Category, decimal> result = new Dictionary<Category, decimal>();
        var bankAccounts = _bankAccountRepository.GetAll();
            
        foreach (var bankAccount in bankAccounts)
        {
            var categoryGroups = _analyticsService.GroupOperationsByCategory(bankAccount.Id, startDate, endDate);
                
            foreach (var categoryGroup in categoryGroups)
            {
                if (result.ContainsKey(categoryGroup.Key))
                {
                    result[categoryGroup.Key] += categoryGroup.Value;
                }
                else
                {
                    result[categoryGroup.Key] = categoryGroup.Value;
                }
            }
        }
            
        return result;
    }

    public decimal CalculateIncomeExpenseDifference(Guid bankAccountId, DateTime startDate, DateTime endDate)
    {
        var bankAccount = _bankAccountRepository.GetById(bankAccountId);
        if (bankAccount == null)
        {
            throw new ArgumentException($"Bank account with ID {bankAccountId} not found");
        }

        return _analyticsService.CalculateIncomeExpenseDifference(bankAccountId, startDate, endDate);
    }

    public Dictionary<Category, decimal> GroupOperationsByCategory(Guid bankAccountId, DateTime startDate, DateTime endDate)
    {
        var bankAccount = _bankAccountRepository.GetById(bankAccountId);
        if (bankAccount == null)
        {
            throw new ArgumentException($"Bank account with ID {bankAccountId} not found");
        }

        return _analyticsService.GroupOperationsByCategory(bankAccountId, startDate, endDate);
    }

    public decimal CalculateTotalIncome(Guid bankAccountId, DateTime startDate, DateTime endDate)
    {
        var bankAccount = _bankAccountRepository.GetById(bankAccountId);
        if (bankAccount == null)
        {
            throw new ArgumentException($"Bank account with ID {bankAccountId} not found");
        }

        return _analyticsService.CalculateTotalIncome(bankAccountId, startDate, endDate);
    }
        
    public decimal CalculateTotalExpenses(Guid bankAccountId, DateTime startDate, DateTime endDate)
    {
        var bankAccount = _bankAccountRepository.GetById(bankAccountId);
        if (bankAccount == null)
        {
            throw new ArgumentException($"Bank account with ID {bankAccountId} not found");
        }

        return _analyticsService.CalculateTotalExpenses(bankAccountId, startDate, endDate);
    }
}