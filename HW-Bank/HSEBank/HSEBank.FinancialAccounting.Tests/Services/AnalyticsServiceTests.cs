using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;
using HSEBank.FinancialAccounting.Services;
using Moq;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Services;

public class AnalyticsServiceTests
{
    private readonly Mock<IOperationRepository> _mockOperationRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly AnalyticsService _service;
    private readonly Guid _bankAccountId;
    private readonly DateTime _startDate;
    private readonly DateTime _endDate;
    private readonly List<Operation> _operations;
    private readonly List<Category> _categories;

    public AnalyticsServiceTests()
    {
        _mockOperationRepository = new Mock<IOperationRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _service = new AnalyticsService(_mockOperationRepository.Object, _mockCategoryRepository.Object);
            
        _bankAccountId = Guid.NewGuid();
        _startDate = new DateTime(2025, 1, 1);
        _endDate = new DateTime(2025, 1, 31);
            
        var incomeCategory1 = new Category(Guid.NewGuid(), CategoryType.Income, "Salary");
        var incomeCategory2 = new Category(Guid.NewGuid(), CategoryType.Income, "Cashback");
        var expenseCategory1 = new Category(Guid.NewGuid(), CategoryType.Expense, "Cafe");
        var expenseCategory2 = new Category(Guid.NewGuid(), CategoryType.Expense, "Transport");
            
        _categories = new List<Category> { incomeCategory1, incomeCategory2, expenseCategory1, expenseCategory2 };
            
        _operations = new List<Operation>
        {
            new(Guid.NewGuid(), OperationType.Income, _bankAccountId, 50m, new DateTime(2025, 1, 15), incomeCategory1.Id, "Salary"),
            new(Guid.NewGuid(), OperationType.Income, _bankAccountId, 2m, new DateTime(2025, 1, 20), incomeCategory2.Id, "Cashback"),
            new(Guid.NewGuid(), OperationType.Expense, _bankAccountId, 1000, new DateTime(2025, 1, 10), expenseCategory1.Id, "Lunch"),
            new(Guid.NewGuid(), OperationType.Expense, _bankAccountId, 337m, new DateTime(2025, 1, 25), expenseCategory2.Id, "Taxi")
        };
            
        _mockOperationRepository.Setup(r => r.GetByBankAccountAndDateRange(_bankAccountId, _startDate, _endDate))
            .Returns(_operations);
            
        foreach (var category in _categories)
        {
            _mockCategoryRepository.Setup(r => r.GetById(category.Id)).Returns(category);
        }
    }

    [Fact]
    public void Constructor_WithNullOperationRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AnalyticsService(null, _mockCategoryRepository.Object));
    }

    [Fact]
    public void Constructor_WithNullCategoryRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AnalyticsService(_mockOperationRepository.Object, null));
    }

    [Fact]
    public void CalculateIncomeExpenseDifference_ReturnsCorrectDifference()
    {
        // Arrange
        decimal expectedIncome = 52m; // 50 + 2
        decimal expectedExpenses = 1337m; // 1000 + 337
        decimal expectedDifference = expectedIncome - expectedExpenses;

        // Act
        var result = _service.CalculateIncomeExpenseDifference(_bankAccountId, _startDate, _endDate);

        // Assert
        Assert.Equal(expectedDifference, result);
        _mockOperationRepository.Verify(r => r.GetByBankAccountAndDateRange(_bankAccountId, _startDate, _endDate), Times.Exactly(2));
    }

    [Fact]
    public void CalculateTotalIncome_ReturnsCorrectTotal()
    {
        // Arrange
        decimal expectedIncome = 52m; // 50 + 2

        // Act
        var result = _service.CalculateTotalIncome(_bankAccountId, _startDate, _endDate);

        // Assert
        Assert.Equal(expectedIncome, result);
        _mockOperationRepository.Verify(r => r.GetByBankAccountAndDateRange(_bankAccountId, _startDate, _endDate), Times.Once);
    }

    [Fact]
    public void CalculateTotalExpenses_ReturnsCorrectTotal()
    {
        // Arrange
        decimal expectedExpenses = 1337m; // 1000 + 337

        // Act
        var result = _service.CalculateTotalExpenses(_bankAccountId, _startDate, _endDate);

        // Assert
        Assert.Equal(expectedExpenses, result);
        _mockOperationRepository.Verify(r => r.GetByBankAccountAndDateRange(_bankAccountId, _startDate, _endDate), Times.Once);
    }

    [Fact]
    public void GroupOperationsByCategory_ReturnsCorrectGrouping()
    {
        // Act
        var result = _service.GroupOperationsByCategory(_bankAccountId, _startDate, _endDate);

        // Assert
        Assert.Equal(4, result.Count);
            
        foreach (var category in _categories)
        {
            var operations = _operations.Where(o => o.CategoryId == category.Id);
            var expectedTotal = operations.Sum(o => o.Amount);
                
            Assert.True(result.ContainsKey(category));
            Assert.Equal(expectedTotal, result[category]);
        }
            
        _mockOperationRepository.Verify(r => r.GetByBankAccountAndDateRange(_bankAccountId, _startDate, _endDate), Times.Once);
            
        foreach (var operation in _operations)
        {
            _mockCategoryRepository.Verify(r => r.GetById(operation.CategoryId), Times.Once);
        }
    }

    [Fact]
    public void GroupOperationsByCategory_WhenCategoryNotFound_SkipsThatOperation()
    {
        // Arrange
        var nonExistentCategoryId = Guid.NewGuid();
        var operationWithNonExistentCategory = new Operation(
            Guid.NewGuid(), 
            OperationType.Income, 
            _bankAccountId, 
            1000m, 
            new DateTime(2025, 1, 5), 
            nonExistentCategoryId, 
            "Test");
            
        var allOperations = new List<Operation>(_operations) { operationWithNonExistentCategory };
            
        _mockOperationRepository.Setup(r => r.GetByBankAccountAndDateRange(_bankAccountId, _startDate, _endDate))
            .Returns(allOperations);
            
        _mockCategoryRepository.Setup(r => r.GetById(nonExistentCategoryId)).Returns((Category)null);

        // Act
        var result = _service.GroupOperationsByCategory(_bankAccountId, _startDate, _endDate);

        // Assert
        Assert.Equal(4, result.Count);
            
        foreach (var category in _categories)
        {
            var operations = _operations.Where(o => o.CategoryId == category.Id);
            var expectedTotal = operations.Sum(o => o.Amount);
                
            Assert.True(result.ContainsKey(category));
            Assert.Equal(expectedTotal, result[category]);
        }
    }
}