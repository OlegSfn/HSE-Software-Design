using HSEBank.FinancialAccounting.Factories;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Factories;

public class BankAccountFactoryTests
{
    private readonly BankAccountFactory _factory;

    public BankAccountFactoryTests()
    {
        _factory = new BankAccountFactory();
    }

    [Fact]
    public void Create_WithValidParameters_ReturnsBankAccount()
    {
        // Arrange
        string name = "Test Account";
        decimal initialBalance = 1000m;

        // Act
        var bankAccount = _factory.Create(name, initialBalance);

        // Assert
        Assert.NotNull(bankAccount);
        Assert.Equal(name, bankAccount.Name);
        Assert.Equal(initialBalance, bankAccount.Balance);
        Assert.NotEqual(Guid.Empty, bankAccount.Id);
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        string name = string.Empty;
        decimal initialBalance = 1000m;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _factory.Create(name, initialBalance));
        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Create_WithNegativeBalance_ThrowsArgumentException()
    {
        // Arrange
        string name = "Test Account";
        decimal initialBalance = -1000m;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _factory.Create(name, initialBalance));
        Assert.Contains("balance", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateWithId_WithValidParameters_ReturnsBankAccount()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        string name = "Test Account";
        decimal balance = 1000m;

        // Act
        var bankAccount = _factory.CreateWithId(id, name, balance);

        // Assert
        Assert.NotNull(bankAccount);
        Assert.Equal(id, bankAccount.Id);
        Assert.Equal(name, bankAccount.Name);
        Assert.Equal(balance, bankAccount.Balance);
    }

    [Fact]
    public void CreateWithId_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        string name = string.Empty;
        decimal balance = 1000m;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _factory.CreateWithId(id, name, balance));
        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CreateWithId_WithNegativeBalance_ThrowsArgumentException()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        string name = "Test Account";
        decimal balance = -1000m;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _factory.CreateWithId(id, name, balance));
        Assert.Contains("balance", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}