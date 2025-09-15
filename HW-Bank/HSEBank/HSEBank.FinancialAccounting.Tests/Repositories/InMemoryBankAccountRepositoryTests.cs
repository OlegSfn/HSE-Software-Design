using HSEBank.FinancialAccounting.Factories;
using HSEBank.FinancialAccounting.Repositories;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Repositories;

public class InMemoryBankAccountRepositoryTests
{
    private readonly InMemoryBankAccountRepository _repository;
    private readonly BankAccountFactory _factory;

    public InMemoryBankAccountRepositoryTests()
    {
        _repository = new InMemoryBankAccountRepository();
        _factory = new BankAccountFactory();
    }

    [Fact]
    public void GetAll_WhenRepositoryIsEmpty_ReturnsEmptyCollection()
    {
        // Act
        var result = _repository.GetAll();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetAll_WhenRepositoryHasItems_ReturnsAllItems()
    {
        // Arrange
        var bankAccount1 = _factory.Create("Account 1", 1000m);
        var bankAccount2 = _factory.Create("Account 2", 2000m);
        _repository.Add(bankAccount1);
        _repository.Add(bankAccount2);

        // Act
        var result = _repository.GetAll();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(bankAccount1, result);
        Assert.Contains(bankAccount2, result);
    }

    [Fact]
    public void GetById_WhenItemExists_ReturnsItem()
    {
        // Arrange
        var bankAccount = _factory.Create("Test Account", 1000m);
        _repository.Add(bankAccount);

        // Act
        var result = _repository.GetById(bankAccount.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bankAccount.Id, result.Id);
        Assert.Equal(bankAccount.Name, result.Name);
        Assert.Equal(bankAccount.Balance, result.Balance);
    }

    [Fact]
    public void GetById_WhenItemDoesNotExist_ReturnsNull()
    {
        // Act
        var result = _repository.GetById(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetByName_WhenItemExists_ReturnsItem()
    {
        // Arrange
        var bankAccount = _factory.Create("Test Account", 1000m);
        _repository.Add(bankAccount);

        // Act
        var result = _repository.GetByName(bankAccount.Name);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bankAccount.Id, result.Id);
        Assert.Equal(bankAccount.Name, result.Name);
        Assert.Equal(bankAccount.Balance, result.Balance);
    }

    [Fact]
    public void GetByName_WhenItemDoesNotExist_ReturnsNull()
    {
        // Act
        var result = _repository.GetByName("Non-existent Account");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Add_WithValidItem_AddsItemToRepository()
    {
        // Arrange
        var bankAccount = _factory.Create("Test Account", 1000m);

        // Act
        _repository.Add(bankAccount);
        var result = _repository.GetById(bankAccount.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(bankAccount.Id, result.Id);
    }

    [Fact]
    public void Add_WithNullItem_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _repository.Add(null));
    }

    [Fact]
    public void Add_WithDuplicateId_ThrowsArgumentException()
    {
        // Arrange
        var bankAccount = _factory.Create("Test Account", 1000m);
        _repository.Add(bankAccount);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _repository.Add(bankAccount));
    }

    [Fact]
    public void Update_WithValidItem_UpdatesItemInRepository()
    {
        // Arrange
        var bankAccount = _factory.Create("Test Account", 1000m);
        _repository.Add(bankAccount);

        // Act
        bankAccount.UpdateName("Updated Account");
        bankAccount.UpdateBalance(2000m);
        _repository.Update(bankAccount);
        var result = _repository.GetById(bankAccount.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Account", result.Name);
        Assert.Equal(2000m, result.Balance);
    }

    [Fact]
    public void Update_WithNullItem_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _repository.Update(null));
    }

    [Fact]
    public void Update_WithNonExistentItem_ThrowsArgumentException()
    {
        // Arrange
        var bankAccount = _factory.Create("Test Account", 1000m);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _repository.Update(bankAccount));
    }

    [Fact]
    public void Delete_WhenItemExists_RemovesItemFromRepository()
    {
        // Arrange
        var bankAccount = _factory.Create("Test Account", 1000m);
        _repository.Add(bankAccount);

        // Act
        var result = _repository.Delete(bankAccount.Id);
        var retrievedAccount = _repository.GetById(bankAccount.Id);

        // Assert
        Assert.True(result);
        Assert.Null(retrievedAccount);
    }

    [Fact]
    public void Delete_WhenItemDoesNotExist_ReturnsFalse()
    {
        // Act
        var result = _repository.Delete(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }
}