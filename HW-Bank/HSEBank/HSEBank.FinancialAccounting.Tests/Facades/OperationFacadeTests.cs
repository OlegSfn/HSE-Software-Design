using HSEBank.FinancialAccounting.Facades;
using HSEBank.FinancialAccounting.Factories;
using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;
using Moq;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Facades;

public class OperationFacadeTests
{
    private readonly Mock<IOperationRepository> _mockOperationRepository;
    private readonly Mock<IBankAccountRepository> _mockBankAccountRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly OperationFactory _operationFactory;
    private readonly OperationFacade _facade;
    private readonly Operation _testOperation;
    private readonly BankAccount _testBankAccount;
    private readonly Category _testIncomeCategory;
    private readonly Category _testExpenseCategory;

    public OperationFacadeTests()
    {
        _mockOperationRepository = new Mock<IOperationRepository>();
        _mockBankAccountRepository = new Mock<IBankAccountRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _operationFactory = new OperationFactory();
            
        _facade = new OperationFacade(
            _mockOperationRepository.Object,
            _mockBankAccountRepository.Object,
            _mockCategoryRepository.Object,
            _operationFactory);

        _testBankAccount = new BankAccount(Guid.NewGuid(), "Test Account", 5000m);
        _testIncomeCategory = new Category(Guid.NewGuid(), CategoryType.Income, "Income Category");
        _testExpenseCategory = new Category(Guid.NewGuid(), CategoryType.Expense, "Expense Category");
        _testOperation = new Operation(
            Guid.NewGuid(),
            OperationType.Income,
            _testBankAccount.Id,
            1000m,
            DateTime.Now,
            _testIncomeCategory.Id,
            "Test operation");

        _mockBankAccountRepository.Setup(r => r.GetById(_testBankAccount.Id)).Returns(_testBankAccount);
        _mockCategoryRepository.Setup(r => r.GetById(_testIncomeCategory.Id)).Returns(_testIncomeCategory);
        _mockCategoryRepository.Setup(r => r.GetById(_testExpenseCategory.Id)).Returns(_testExpenseCategory);
    }

    [Fact]
    public void Constructor_WithNullOperationRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OperationFacade(
            null,
            _mockBankAccountRepository.Object,
            _mockCategoryRepository.Object,
            _operationFactory));
    }

    [Fact]
    public void Constructor_WithNullBankAccountRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OperationFacade(
            _mockOperationRepository.Object,
            null,
            _mockCategoryRepository.Object,
            _operationFactory));
    }

    [Fact]
    public void Constructor_WithNullCategoryRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OperationFacade(
            _mockOperationRepository.Object,
            _mockBankAccountRepository.Object,
            null,
            _operationFactory));
    }

    [Fact]
    public void Constructor_WithNullOperationFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new OperationFacade(
            _mockOperationRepository.Object,
            _mockBankAccountRepository.Object,
            _mockCategoryRepository.Object,
            null));
    }

    [Fact]
    public void GetAllOperations_CallsRepositoryGetAll()
    {
        // Arrange
        var operations = new[] { _testOperation };
        _mockOperationRepository.Setup(r => r.GetAll()).Returns(operations);

        // Act
        var result = _facade.GetAllOperations();

        // Assert
        Assert.Same(operations, result);
        _mockOperationRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void GetOperationById_CallsRepositoryGetById()
    {
        // Arrange
        _mockOperationRepository.Setup(r => r.GetById(_testOperation.Id)).Returns(_testOperation);

        // Act
        var result = _facade.GetOperationById(_testOperation.Id);

        // Assert
        Assert.Same(_testOperation, result);
        _mockOperationRepository.Verify(r => r.GetById(_testOperation.Id), Times.Once);
    }

    [Fact]
    public void GetOperationsByBankAccount_CallsRepositoryGetByBankAccount()
    {
        // Arrange
        var operations = new[] { _testOperation };
        _mockOperationRepository.Setup(r => r.GetByBankAccount(_testBankAccount.Id)).Returns(operations);

        // Act
        var result = _facade.GetOperationsByBankAccount(_testBankAccount.Id);

        // Assert
        Assert.Same(operations, result);
        _mockOperationRepository.Verify(r => r.GetByBankAccount(_testBankAccount.Id), Times.Once);
    }

    [Fact]
    public void GetOperationsByCategory_CallsRepositoryGetByCategory()
    {
        // Arrange
        var operations = new[] { _testOperation };
        _mockOperationRepository.Setup(r => r.GetByCategory(_testIncomeCategory.Id)).Returns(operations);

        // Act
        var result = _facade.GetOperationsByCategory(_testIncomeCategory.Id);

        // Assert
        Assert.Same(operations, result);
        _mockOperationRepository.Verify(r => r.GetByCategory(_testIncomeCategory.Id), Times.Once);
    }

    [Fact]
    public void GetOperationsByType_CallsRepositoryGetByType()
    {
        // Arrange
        var operations = new[] { _testOperation };
        _mockOperationRepository.Setup(r => r.GetByType(OperationType.Income)).Returns(operations);

        // Act
        var result = _facade.GetOperationsByType(OperationType.Income);

        // Assert
        Assert.Same(operations, result);
        _mockOperationRepository.Verify(r => r.GetByType(OperationType.Income), Times.Once);
    }

    [Fact]
    public void GetOperationsByDateRange_CallsRepositoryGetByDateRange()
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(-30);
        var endDate = DateTime.Now;
        var operations = new[] { _testOperation };
        _mockOperationRepository.Setup(r => r.GetByDateRange(startDate, endDate)).Returns(operations);

        // Act
        var result = _facade.GetOperationsByDateRange(startDate, endDate);

        // Assert
        Assert.Same(operations, result);
        _mockOperationRepository.Verify(r => r.GetByDateRange(startDate, endDate), Times.Once);
    }

    [Fact]
    public void GetOperationsByBankAccountAndDateRange_CallsRepositoryGetByBankAccountAndDateRange()
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(-30);
        var endDate = DateTime.Now;
        var operations = new[] { _testOperation };
        _mockOperationRepository.Setup(r => r.GetByBankAccountAndDateRange(
            _testBankAccount.Id, startDate, endDate)).Returns(operations);

        // Act
        var result = _facade.GetOperationsByBankAccountAndDateRange(_testBankAccount.Id, startDate, endDate);

        // Assert
        Assert.Same(operations, result);
        _mockOperationRepository.Verify(r => r.GetByBankAccountAndDateRange(
            _testBankAccount.Id, startDate, endDate), Times.Once);
    }

    [Fact]
    public void CreateOperation_Income_IncreasesBalance()
    {
        // Arrange
        decimal initialBalance = _testBankAccount.Balance;
        decimal amount = 1000m;

        // Act
        var result = _facade.CreateOperation(
            OperationType.Income,
            _testBankAccount.Id,
            amount,
            DateTime.Now,
            _testIncomeCategory.Id,
            "Test operation");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OperationType.Income, result.Type);
        Assert.Equal(_testBankAccount.Id, result.BankAccountId);
        Assert.Equal(amount, result.Amount);
        Assert.Equal(_testIncomeCategory.Id, result.CategoryId);
        Assert.Equal("Test operation", result.Description);

        _mockOperationRepository.Verify(r => r.Add(It.IsAny<Operation>()), Times.Once);
        _mockBankAccountRepository.Verify(r => r.Update(_testBankAccount), Times.Once);
        Assert.Equal(initialBalance + amount, _testBankAccount.Balance);
    }

    [Fact]
    public void CreateOperation_Expense_DecreasesBalance()
    {
        // Arrange
        decimal initialBalance = _testBankAccount.Balance;
        decimal amount = 1000m;

        // Act
        var result = _facade.CreateOperation(
            OperationType.Expense,
            _testBankAccount.Id,
            amount,
            DateTime.Now,
            _testExpenseCategory.Id,
            "Test operation");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(OperationType.Expense, result.Type);
        Assert.Equal(_testBankAccount.Id, result.BankAccountId);
        Assert.Equal(amount, result.Amount);
        Assert.Equal(_testExpenseCategory.Id, result.CategoryId);
        Assert.Equal("Test operation", result.Description);

        _mockOperationRepository.Verify(r => r.Add(It.IsAny<Operation>()), Times.Once);
        _mockBankAccountRepository.Verify(r => r.Update(_testBankAccount), Times.Once);
        Assert.Equal(initialBalance - amount, _testBankAccount.Balance);
    }

    [Fact]
    public void CreateOperation_WithBankAccountNotFound_ThrowsArgumentException()
    {
        // Arrange
        Guid nonExistentBankAccountId = Guid.NewGuid();
        _mockBankAccountRepository.Setup(r => r.GetById(nonExistentBankAccountId)).Returns((BankAccount)null);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _facade.CreateOperation(
            OperationType.Income,
            nonExistentBankAccountId,
            1000m,
            DateTime.Now,
            _testIncomeCategory.Id,
            "Test operation"));

        Assert.Contains(nonExistentBankAccountId.ToString(), exception.Message);
    }

    [Fact]
    public void CreateOperation_WithCategoryNotFound_ThrowsArgumentException()
    {
        // Arrange
        Guid nonExistentCategoryId = Guid.NewGuid();
        _mockCategoryRepository.Setup(r => r.GetById(nonExistentCategoryId)).Returns((Category)null);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _facade.CreateOperation(
            OperationType.Income,
            _testBankAccount.Id,
            1000m,
            DateTime.Now,
            nonExistentCategoryId,
            "Test operation"));

        Assert.Contains(nonExistentCategoryId.ToString(), exception.Message);
    }

    [Fact]
    public void CreateOperation_IncomeCategoryMismatch_ThrowsArgumentException()
    {
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _facade.CreateOperation(
            OperationType.Income,
            _testBankAccount.Id,
            1000m,
            DateTime.Now,
            _testExpenseCategory.Id,
            "Test operation"));

        Assert.Contains("Income operation must have an income category", exception.Message);
    }

    [Fact]
    public void CreateOperation_ExpenseCategoryMismatch_ThrowsArgumentException()
    {
        // Arrange
            
        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _facade.CreateOperation(
            OperationType.Expense,
            _testBankAccount.Id,
            1000m,
            DateTime.Now,
            _testIncomeCategory.Id,
            "Test operation"));

        Assert.Contains("Expense operation must have an expense category", exception.Message);
    }

    [Fact]
    public void DeleteOperation_Income_DecreasesBalance()
    {
        // Arrange
        decimal initialBalance = _testBankAccount.Balance;
            
        var operation = new Operation(
            Guid.NewGuid(),
            OperationType.Income,
            _testBankAccount.Id,
            1000m,
            DateTime.Now,
            _testIncomeCategory.Id,
            "Test operation");
            
        _mockOperationRepository.Setup(r => r.GetById(operation.Id)).Returns(operation);
        _mockOperationRepository.Setup(r => r.Delete(operation.Id)).Returns(true);

        // Act
        var result = _facade.DeleteOperation(operation.Id);

        // Assert
        Assert.True(result);
        _mockOperationRepository.Verify(r => r.Delete(operation.Id), Times.Once);
        _mockBankAccountRepository.Verify(r => r.Update(_testBankAccount), Times.Once);
        Assert.Equal(initialBalance - operation.Amount, _testBankAccount.Balance);
    }

    [Fact]
    public void DeleteOperation_Expense_IncreasesBalance()
    {
        // Arrange
        decimal initialBalance = _testBankAccount.Balance;
            
        var operation = new Operation(
            Guid.NewGuid(),
            OperationType.Expense,
            _testBankAccount.Id,
            1000m,
            DateTime.Now,
            _testExpenseCategory.Id,
            "Test operation");
            
        _mockOperationRepository.Setup(r => r.GetById(operation.Id)).Returns(operation);
        _mockOperationRepository.Setup(r => r.Delete(operation.Id)).Returns(true);

        // Act
        var result = _facade.DeleteOperation(operation.Id);

        // Assert
        Assert.True(result);
        _mockOperationRepository.Verify(r => r.Delete(operation.Id), Times.Once);
        _mockBankAccountRepository.Verify(r => r.Update(_testBankAccount), Times.Once);
        Assert.Equal(initialBalance + operation.Amount, _testBankAccount.Balance);
    }

    [Fact]
    public void DeleteOperation_OperationNotFound_ReturnsFalse()
    {
        // Arrange
        Guid nonExistentOperationId = Guid.NewGuid();
        _mockOperationRepository.Setup(r => r.GetById(nonExistentOperationId)).Returns((Operation)null);

        // Act
        var result = _facade.DeleteOperation(nonExistentOperationId);

        // Assert
        Assert.False(result);
        _mockOperationRepository.Verify(r => r.Delete(nonExistentOperationId), Times.Never);
        _mockBankAccountRepository.Verify(r => r.Update(It.IsAny<BankAccount>()), Times.Never);
    }
}