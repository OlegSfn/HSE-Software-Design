using HSEBank.FinancialAccounting.Factories;
using HSEBank.FinancialAccounting.Models.Enums;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Factories;

public class OperationFactoryTests
{
    private readonly OperationFactory _factory;

    public OperationFactoryTests()
    {
        _factory = new OperationFactory();
    }

    [Fact]
    public void Create_WithValidParameters_ReturnsOperation()
    {
        // Arrange
        OperationType type = OperationType.Income;
        Guid bankAccountId = Guid.NewGuid();
        decimal amount = 1000m;
        DateTime date = DateTime.Now;
        Guid categoryId = Guid.NewGuid();
        string description = "Test Operation";

        // Act
        var operation = _factory.Create(type, bankAccountId, amount, date, categoryId, description);

        // Assert
        Assert.NotNull(operation);
        Assert.Equal(type, operation.Type);
        Assert.Equal(bankAccountId, operation.BankAccountId);
        Assert.Equal(amount, operation.Amount);
        Assert.Equal(date, operation.Date);
        Assert.Equal(categoryId, operation.CategoryId);
        Assert.Equal(description, operation.Description);
        Assert.NotEqual(Guid.Empty, operation.Id);
    }

    [Fact]
    public void Create_WithZeroAmount_ThrowsArgumentException()
    {
        // Arrange
        OperationType type = OperationType.Income;
        Guid bankAccountId = Guid.NewGuid();
        decimal amount = 0m;
        DateTime date = DateTime.Now;
        Guid categoryId = Guid.NewGuid();
        string description = "Test Operation";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            _factory.Create(type, bankAccountId, amount, date, categoryId, description));
        Assert.Contains("amount", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_WithNegativeAmount_ThrowsArgumentException()
    {
        // Arrange
        OperationType type = OperationType.Income;
        Guid bankAccountId = Guid.NewGuid();
        decimal amount = -1000m;
        DateTime date = DateTime.Now;
        Guid categoryId = Guid.NewGuid();
        string description = "Test Operation";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            _factory.Create(type, bankAccountId, amount, date, categoryId, description));
        Assert.Contains("amount", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_WithNullDescription_ReturnsOperationWithNullDescription()
    {
        // Arrange
        OperationType type = OperationType.Income;
        Guid bankAccountId = Guid.NewGuid();
        decimal amount = 1000m;
        DateTime date = DateTime.Now;
        Guid categoryId = Guid.NewGuid();
        string description = null;

        // Act
        var operation = _factory.Create(type, bankAccountId, amount, date, categoryId, description);

        // Assert
        Assert.Null(operation.Description);
    }

    [Theory]
    [InlineData(OperationType.Income)]
    [InlineData(OperationType.Expense)]
    public void Create_WithDifferentTypes_ReturnsOperationWithCorrectType(OperationType type)
    {
        // Arrange
        Guid bankAccountId = Guid.NewGuid();
        decimal amount = 1000m;
        DateTime date = DateTime.Now;
        Guid categoryId = Guid.NewGuid();
        string description = "Test Operation";

        // Act
        var operation = _factory.Create(type, bankAccountId, amount, date, categoryId, description);

        // Assert
        Assert.Equal(type, operation.Type);
    }

    [Fact]
    public void CreateWithId_WithValidParameters_ReturnsOperation()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        OperationType type = OperationType.Income;
        Guid bankAccountId = Guid.NewGuid();
        decimal amount = 1000m;
        DateTime date = DateTime.Now;
        Guid categoryId = Guid.NewGuid();
        string description = "Test Operation";

        // Act
        var operation = _factory.CreateWithId(id, type, bankAccountId, amount, date, categoryId, description);

        // Assert
        Assert.NotNull(operation);
        Assert.Equal(id, operation.Id);
        Assert.Equal(type, operation.Type);
        Assert.Equal(bankAccountId, operation.BankAccountId);
        Assert.Equal(amount, operation.Amount);
        Assert.Equal(date, operation.Date);
        Assert.Equal(categoryId, operation.CategoryId);
        Assert.Equal(description, operation.Description);
    }

    [Fact]
    public void CreateWithId_WithZeroAmount_ThrowsArgumentException()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        OperationType type = OperationType.Income;
        Guid bankAccountId = Guid.NewGuid();
        decimal amount = 0m;
        DateTime date = DateTime.Now;
        Guid categoryId = Guid.NewGuid();
        string description = "Test Operation";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => 
            _factory.CreateWithId(id, type, bankAccountId, amount, date, categoryId, description));
        Assert.Contains("amount", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}