using HSEBank.FinancialAccounting.Facades;
using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Interfaces.Services;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;
using Moq;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Facades;

public class AnalyticsFacadeTests
{
    private readonly Mock<IAnalyticsService> _mockAnalyticsService;
    private readonly Mock<IBankAccountRepository> _mockBankAccountRepository;
    private readonly AnalyticsFacade _analyticsFacade;
    private readonly Guid _bankAccountId = Guid.NewGuid();
    private readonly BankAccount _testBankAccount;
    private readonly DateTime _startDate = new DateTime(2023, 1, 1);
    private readonly DateTime _endDate = new DateTime(2023, 12, 31);

    public AnalyticsFacadeTests()
    {
        _mockAnalyticsService = new Mock<IAnalyticsService>();
        _mockBankAccountRepository = new Mock<IBankAccountRepository>();
            
        _analyticsFacade = new AnalyticsFacade(
            _mockAnalyticsService.Object,
            _mockBankAccountRepository.Object);
                
        _testBankAccount = new BankAccount(_bankAccountId, "Test Account", 1000m);
        _mockBankAccountRepository.Setup(r => r.GetById(_bankAccountId)).Returns(_testBankAccount);
    }

    [Fact]
    public void Constructor_WithNullAnalyticsService_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AnalyticsFacade(
            null,
            _mockBankAccountRepository.Object));
    }

    [Fact]
    public void Constructor_WithNullBankAccountRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AnalyticsFacade(
            _mockAnalyticsService.Object,
            null));
    }

    [Fact]
    public void CalculateTotalIncome_AllBankAccounts_CallsAnalyticsService()
    {
        // Arrange
        _mockAnalyticsService.Setup(s => s.CalculateTotalIncome(
                It.IsAny<Guid>(), _startDate, _endDate))
            .Returns(1000m);
                
        var bankAccounts = new List<BankAccount> 
        { 
            _testBankAccount,
            new BankAccount(Guid.NewGuid(), "Account 2", 500m)
        };
            
        _mockBankAccountRepository.Setup(r => r.GetAll()).Returns(bankAccounts);

        // Act
        var result = _analyticsFacade.CalculateTotalIncome(_startDate, _endDate);

        // Assert
        _mockBankAccountRepository.Verify(r => r.GetAll(), Times.Once);
        _mockAnalyticsService.Verify(s => s.CalculateTotalIncome(
            It.IsAny<Guid>(), _startDate, _endDate), Times.Exactly(2));
            
        Assert.Equal(2000m, result); // 1000m * 2 accounts
    }

    [Fact]
    public void CalculateTotalExpenses_AllBankAccounts_CallsAnalyticsService()
    {
        // Arrange
        _mockAnalyticsService.Setup(s => s.CalculateTotalExpenses(
                It.IsAny<Guid>(), _startDate, _endDate))
            .Returns(500m);
                
        var bankAccounts = new List<BankAccount> 
        { 
            _testBankAccount,
            new BankAccount(Guid.NewGuid(), "Account 2", 500m)
        };
            
        _mockBankAccountRepository.Setup(r => r.GetAll()).Returns(bankAccounts);

        // Act
        var result = _analyticsFacade.CalculateTotalExpenses(_startDate, _endDate);

        // Assert
        _mockBankAccountRepository.Verify(r => r.GetAll(), Times.Once);
        _mockAnalyticsService.Verify(s => s.CalculateTotalExpenses(
            It.IsAny<Guid>(), _startDate, _endDate), Times.Exactly(2));
            
        Assert.Equal(1000m, result); // 500m * 2 accounts
    }

    [Fact]
    public void CalculateIncomeExpenseDifference_AllBankAccounts_ReturnsCorrectDifference()
    {
        // Arrange
        _mockAnalyticsService.Setup(s => s.CalculateTotalIncome(
                It.IsAny<Guid>(), _startDate, _endDate))
            .Returns(1000m);
                
        _mockAnalyticsService.Setup(s => s.CalculateTotalExpenses(
                It.IsAny<Guid>(), _startDate, _endDate))
            .Returns(400m);
                
        var bankAccounts = new List<BankAccount> 
        { 
            _testBankAccount,
            new BankAccount(Guid.NewGuid(), "Account 2", 500m)
        };
            
        _mockBankAccountRepository.Setup(r => r.GetAll()).Returns(bankAccounts);

        // Act
        var result = _analyticsFacade.CalculateIncomeExpenseDifference(_startDate, _endDate);

        // Assert
        Assert.Equal(1200m, result); // (1000m * 2) - (400m * 2) = 2000 - 800 = 1200
    }

    [Fact]
    public void CalculateTotalIncome_SpecificBankAccount_CallsAnalyticsService()
    {
        // Arrange
        _mockAnalyticsService.Setup(s => s.CalculateTotalIncome(
                _bankAccountId, _startDate, _endDate))
            .Returns(1000m);

        // Act
        var result = _analyticsFacade.CalculateTotalIncome(_bankAccountId, _startDate, _endDate);

        // Assert
        _mockBankAccountRepository.Verify(r => r.GetById(_bankAccountId), Times.Once);
        _mockAnalyticsService.Verify(s => s.CalculateTotalIncome(
            _bankAccountId, _startDate, _endDate), Times.Once);
        Assert.Equal(1000m, result);
    }

    [Fact]
    public void CalculateTotalIncome_NonExistentBankAccount_ThrowsArgumentException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _mockBankAccountRepository.Setup(r => r.GetById(nonExistentId)).Returns((BankAccount)null);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _analyticsFacade.CalculateTotalIncome(
            nonExistentId, _startDate, _endDate));
    }

    [Fact]
    public void CalculateTotalExpenses_SpecificBankAccount_CallsAnalyticsService()
    {
        // Arrange
        _mockAnalyticsService.Setup(s => s.CalculateTotalExpenses(
                _bankAccountId, _startDate, _endDate))
            .Returns(500m);

        // Act
        var result = _analyticsFacade.CalculateTotalExpenses(_bankAccountId, _startDate, _endDate);

        // Assert
        _mockBankAccountRepository.Verify(r => r.GetById(_bankAccountId), Times.Once);
        _mockAnalyticsService.Verify(s => s.CalculateTotalExpenses(
            _bankAccountId, _startDate, _endDate), Times.Once);
        Assert.Equal(500m, result);
    }

    [Fact]
    public void CalculateTotalExpenses_NonExistentBankAccount_ThrowsArgumentException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _mockBankAccountRepository.Setup(r => r.GetById(nonExistentId)).Returns((BankAccount)null);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _analyticsFacade.CalculateTotalExpenses(
            nonExistentId, _startDate, _endDate));
    }

    [Fact]
    public void CalculateIncomeExpenseDifference_SpecificBankAccount_CallsAnalyticsService()
    {
        // Arrange
        _mockAnalyticsService.Setup(s => s.CalculateIncomeExpenseDifference(
                _bankAccountId, _startDate, _endDate))
            .Returns(500m);

        // Act
        var result = _analyticsFacade.CalculateIncomeExpenseDifference(_bankAccountId, _startDate, _endDate);

        // Assert
        _mockBankAccountRepository.Verify(r => r.GetById(_bankAccountId), Times.Once);
        _mockAnalyticsService.Verify(s => s.CalculateIncomeExpenseDifference(
            _bankAccountId, _startDate, _endDate), Times.Once);
        Assert.Equal(500m, result);
    }

    [Fact]
    public void CalculateIncomeExpenseDifference_NonExistentBankAccount_ThrowsArgumentException()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _mockBankAccountRepository.Setup(r => r.GetById(nonExistentId)).Returns((BankAccount)null);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _analyticsFacade.CalculateIncomeExpenseDifference(
            nonExistentId, _startDate, _endDate));
    }

    [Fact]
    public void GroupOperationsByCategory_SpecificBankAccount_CallsAnalyticsService()
    {
        // Arrange
        var expectedResult = new Dictionary<Category, decimal>
        {
            { new Category(Guid.NewGuid(), CategoryType.Income, "Salary"), 1000m },
            { new Category(Guid.NewGuid(), CategoryType.Expense, "Food"), 500m }
        };
            
        _mockAnalyticsService.Setup(s => s.GroupOperationsByCategory(
                _bankAccountId, _startDate, _endDate))
            .Returns(expectedResult);

        // Act
        var result = _analyticsFacade.GroupOperationsByCategory(_bankAccountId, _startDate, _endDate);

        // Assert
        _mockBankAccountRepository.Verify(r => r.GetById(_bankAccountId), Times.Once);
        _mockAnalyticsService.Verify(s => s.GroupOperationsByCategory(
            _bankAccountId, _startDate, _endDate), Times.Once);
        Assert.Same(expectedResult, result);
    }

    [Fact]
    public void GroupOperationsByCategory_AllBankAccounts_CallsAnalyticsService()
    {
        // Arrange
        var category1 = new Category(Guid.NewGuid(), CategoryType.Income, "Salary");
        var category2 = new Category(Guid.NewGuid(), CategoryType.Expense, "Food");
            
        var result1 = new Dictionary<Category, decimal>
        {
            { category1, 1000m },
            { category2, 500m }
        };
            
        var result2 = new Dictionary<Category, decimal>
        {
            { category1, 800m },
            { category2, 300m }
        };
            
        var bankAccount2Id = Guid.NewGuid();
        var bankAccounts = new List<BankAccount> 
        { 
            _testBankAccount,
            new(bankAccount2Id, "Account 2", 500m)
        };
            
        _mockBankAccountRepository.Setup(r => r.GetAll()).Returns(bankAccounts);
            
        _mockAnalyticsService.Setup(s => s.GroupOperationsByCategory(
                _bankAccountId, _startDate, _endDate))
            .Returns(result1);
                
        _mockAnalyticsService.Setup(s => s.GroupOperationsByCategory(
                bankAccount2Id, _startDate, _endDate))
            .Returns(result2);

        // Act
        var result = _analyticsFacade.GroupOperationsByCategory(_startDate, _endDate);

        // Assert
        _mockBankAccountRepository.Verify(r => r.GetAll(), Times.Once);
        _mockAnalyticsService.Verify(s => s.GroupOperationsByCategory(
            It.IsAny<Guid>(), _startDate, _endDate), Times.Exactly(2));
            
        Assert.Equal(2, result.Count);
        Assert.Equal(1800m, result[category1]); // 1000m + 800m
        Assert.Equal(800m, result[category2]);  // 500m + 300m
    }
}