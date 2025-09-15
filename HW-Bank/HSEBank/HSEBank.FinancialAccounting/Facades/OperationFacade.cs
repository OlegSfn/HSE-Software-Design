using HSEBank.FinancialAccounting.Interfaces.Facades;
using HSEBank.FinancialAccounting.Interfaces.Factories;
using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.Facades;

public class OperationFacade : IOperationFacade
{
    private readonly IOperationRepository _operationRepository;
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IOperationFactory _operationFactory;

    public OperationFacade(
        IOperationRepository operationRepository,
        IBankAccountRepository bankAccountRepository,
        ICategoryRepository categoryRepository,
        IOperationFactory operationFactory)
    {
        _operationRepository = operationRepository ?? throw new ArgumentNullException(nameof(operationRepository));
        _bankAccountRepository = bankAccountRepository ?? throw new ArgumentNullException(nameof(bankAccountRepository));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        _operationFactory = operationFactory ?? throw new ArgumentNullException(nameof(operationFactory));
    }

    public IEnumerable<Operation> GetAllOperations() => _operationRepository.GetAll();

    public Operation GetOperationById(Guid id) => _operationRepository.GetById(id);

    public IEnumerable<Operation> GetOperationsByBankAccount(Guid bankAccountId) 
        => _operationRepository.GetByBankAccount(bankAccountId);

    public IEnumerable<Operation> GetOperationsByCategory(Guid categoryId) 
        => _operationRepository.GetByCategory(categoryId);

    public IEnumerable<Operation> GetOperationsByType(OperationType type)
        => _operationRepository.GetByType(type);

    public IEnumerable<Operation> GetOperationsByDateRange(DateTime startDate, DateTime endDate)
        => _operationRepository.GetByDateRange(startDate, endDate);

    public IEnumerable<Operation> GetOperationsByBankAccountAndDateRange(Guid bankAccountId, DateTime startDate, DateTime endDate)
        => _operationRepository.GetByBankAccountAndDateRange(bankAccountId, startDate, endDate);

    public Operation CreateOperation(OperationType type, Guid bankAccountId, decimal amount, DateTime date, Guid categoryId, string description = null)
    {
        var bankAccount = _bankAccountRepository.GetById(bankAccountId);
        if (bankAccount == null)
        {
            throw new ArgumentException($"Bank account with ID {bankAccountId} not found");
        }

        var category = _categoryRepository.GetById(categoryId);
        if (category == null)
        {
            throw new ArgumentException($"Category with ID {categoryId} not found");
        }

        if (type == OperationType.Income && category.Type != CategoryType.Income)
        {
            throw new ArgumentException("Income operation must have an income category");
        }

        if (type == OperationType.Expense && category.Type != CategoryType.Expense)
        {
            throw new ArgumentException("Expense operation must have an expense category");
        }

        var operation = _operationFactory.Create(type, bankAccountId, amount, date, categoryId, description);
        _operationRepository.Add(operation);

        if (type == OperationType.Income)
        {
            bankAccount.AddToBalance(amount);
        }
        else
        {
            bankAccount.SubtractFromBalance(amount);
        }

        _bankAccountRepository.Update(bankAccount);

        return operation;
    }

    public Operation UpdateOperationType(Guid id, OperationType type)
    {
        var operation = _operationRepository.GetById(id);
        if (operation == null)
        {
            throw new ArgumentException($"Operation with ID {id} not found");
        }

        var category = _categoryRepository.GetById(operation.CategoryId);
        if (category == null)
        {
            throw new ArgumentException($"Category with ID {operation.CategoryId} not found");
        }

        if (type == OperationType.Income && category.Type != CategoryType.Income)
        {
            throw new ArgumentException("Income operation must have an income category");
        }

        if (type == OperationType.Expense && category.Type != CategoryType.Expense)
        {
            throw new ArgumentException("Expense operation must have an expense category");
        }

        var bankAccount = _bankAccountRepository.GetById(operation.BankAccountId);
        if (bankAccount == null)
        {
            throw new ArgumentException($"Bank account with ID {operation.BankAccountId} not found");
        }

        if (operation.Type == OperationType.Income)
        {
            bankAccount.SubtractFromBalance(operation.Amount);
        }
        else
        {
            bankAccount.AddToBalance(operation.Amount);
        }

        if (type == OperationType.Income)
        {
            bankAccount.AddToBalance(operation.Amount);
        }
        else
        {
            bankAccount.SubtractFromBalance(operation.Amount);
        }

        _bankAccountRepository.Update(bankAccount);

        operation.UpdateType(type);
        _operationRepository.Update(operation);

        return operation;
    }
        
    public Operation UpdateOperationAmount(Guid id, decimal amount)
    {
        var operation = _operationRepository.GetById(id);
        if (operation == null)
        {
            throw new ArgumentException($"Operation with ID {id} not found");
        }

        var bankAccount = _bankAccountRepository.GetById(operation.BankAccountId);
        if (bankAccount == null)
        {
            throw new ArgumentException($"Bank account with ID {operation.BankAccountId} not found");
        }

        if (operation.Type == OperationType.Income)
        {
            bankAccount.SubtractFromBalance(operation.Amount);
        }
        else
        {
            bankAccount.AddToBalance(operation.Amount);
        }

        if (operation.Type == OperationType.Income)
        {
            bankAccount.AddToBalance(amount);
        }
        else
        {
            bankAccount.SubtractFromBalance(amount);
        }

        _bankAccountRepository.Update(bankAccount);

        operation.UpdateAmount(amount);
        _operationRepository.Update(operation);

        return operation;
    }
        
    public Operation UpdateOperationDate(Guid id, DateTime date)
    {
        var operation = _operationRepository.GetById(id);
        if (operation == null)
        {
            throw new ArgumentException($"Operation with ID {id} not found");
        }

        operation.UpdateDate(date);
        _operationRepository.Update(operation);

        return operation;
    }
        
    public Operation UpdateOperationDescription(Guid id, string description)
    {
        var operation = _operationRepository.GetById(id);
        if (operation == null)
        {
            throw new ArgumentException($"Operation with ID {id} not found");
        }

        operation.UpdateDescription(description);
        _operationRepository.Update(operation);

        return operation;
    }
        
    public Operation UpdateOperationCategory(Guid id, Guid categoryId)
    {
        var operation = _operationRepository.GetById(id);
        if (operation == null)
        {
            throw new ArgumentException($"Operation with ID {id} not found");
        }

        var category = _categoryRepository.GetById(categoryId);
        if (category == null)
        {
            throw new ArgumentException($"Category with ID {categoryId} not found");
        }

        if (operation.Type == OperationType.Income && category.Type != CategoryType.Income)
        {
            throw new ArgumentException("Income operation must have an income category");
        }

        if (operation.Type == OperationType.Expense && category.Type != CategoryType.Expense)
        {
            throw new ArgumentException("Expense operation must have an expense category");
        }

        operation.UpdateCategory(categoryId);
        _operationRepository.Update(operation);

        return operation;
    }
        
    public bool DeleteOperation(Guid id)
    {
        var operation = _operationRepository.GetById(id);
        if (operation == null)
        {
            return false;
        }

        var bankAccount = _bankAccountRepository.GetById(operation.BankAccountId);
        if (bankAccount == null)
        {
            throw new ArgumentException($"Bank account with ID {operation.BankAccountId} not found");
        }

        if (operation.Type == OperationType.Income)
        {
            bankAccount.SubtractFromBalance(operation.Amount);
        }
        else
        {
            bankAccount.AddToBalance(operation.Amount);
        }

        _bankAccountRepository.Update(bankAccount);

        return _operationRepository.Delete(id);
    }
}