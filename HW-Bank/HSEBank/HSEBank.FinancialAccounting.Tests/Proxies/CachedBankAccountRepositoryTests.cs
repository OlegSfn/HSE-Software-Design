using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Proxies;
using Moq;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Proxies;

public class CachedBankAccountRepositoryTests
{
    private readonly Mock<IBankAccountRepository> _mockRepository;
    private readonly CachedBankAccountRepository _cachedRepository;
    private readonly List<BankAccount> _testBankAccounts;

    public CachedBankAccountRepositoryTests()
    {
        _mockRepository = new Mock<IBankAccountRepository>();
        _cachedRepository = new CachedBankAccountRepository(_mockRepository.Object);

        _testBankAccounts = new List<BankAccount>
        {
            new(Guid.NewGuid(), "Account 1", 1000m),
            new(Guid.NewGuid(), "Account 2", 2000m),
            new(Guid.NewGuid(), "Account 3", 3000m)
        };

        _mockRepository.Setup(r => r.GetAll()).Returns(_testBankAccounts);
        foreach (var account in _testBankAccounts)
        {
            _mockRepository.Setup(r => r.GetById(account.Id)).Returns(account);
            _mockRepository.Setup(r => r.GetByName(account.Name)).Returns(account);
        }
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CachedBankAccountRepository(null));
    }

    [Fact]
    public void GetAll_InitializesCache_AndReturnsCachedItems()
    {
        // Act
        var result1 = _cachedRepository.GetAll();
        var result2 = _cachedRepository.GetAll();

        // Assert
        Assert.Equal(_testBankAccounts.Count, result1.Count());
        Assert.Equal(_testBankAccounts.Count, result2.Count());
        foreach (var account in _testBankAccounts)
        {
            Assert.Contains(result1, a => a.Id == account.Id);
            Assert.Contains(result2, a => a.Id == account.Id);
        }

        _mockRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void GetById_WithExistingId_ReturnsCachedItem()
    {
        // Arrange
        var existingAccount = _testBankAccounts[0];

        // Act
        var result1 = _cachedRepository.GetById(existingAccount.Id);
        var result2 = _cachedRepository.GetById(existingAccount.Id);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(existingAccount.Id, result1.Id);
        Assert.Equal(existingAccount.Id, result2.Id);

        _mockRepository.Verify(r => r.GetAll(), Times.Once);
        _mockRepository.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void GetById_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = _cachedRepository.GetById(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetByName_WithExistingName_ReturnsCachedItem()
    {
        // Arrange
        var existingAccount = _testBankAccounts[0];

        // Act
        var result1 = _cachedRepository.GetByName(existingAccount.Name);
        var result2 = _cachedRepository.GetByName(existingAccount.Name);

        // Assert
        Assert.NotNull(result1);
        Assert.NotNull(result2);
        Assert.Equal(existingAccount.Id, result1.Id);
        Assert.Equal(existingAccount.Id, result2.Id);

        _mockRepository.Verify(r => r.GetAll(), Times.Once);
        _mockRepository.Verify(r => r.GetByName(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void GetByName_WithNonExistentName_ReturnsNull()
    {
        // Arrange
        var nonExistentName = "Non-existent account";

        // Act
        var result = _cachedRepository.GetByName(nonExistentName);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Add_CallsUnderlyingRepository_AndUpdatesCacheItem()
    {
        // Arrange
        var newAccount = new BankAccount(Guid.NewGuid(), "New Account", 5000m);

        // Act
        _cachedRepository.Add(newAccount);
        var result = _cachedRepository.GetById(newAccount.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(newAccount.Id, result.Id);
        _mockRepository.Verify(r => r.Add(newAccount), Times.Once);
    }

    [Fact]
    public void Add_WithNullAccount_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _cachedRepository.Add(null));
    }

    [Fact]
    public void Update_CallsUnderlyingRepository_AndUpdatesCacheItem()
    {
        // Arrange
        var existingAccount = _testBankAccounts[0];
        var updatedName = "Updated Account";
        existingAccount.UpdateName(updatedName);

        // Act
        _cachedRepository.Update(existingAccount);
        var result = _cachedRepository.GetById(existingAccount.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingAccount.Id, result.Id);
        Assert.Equal(updatedName, result.Name);
        _mockRepository.Verify(r => r.Update(existingAccount), Times.Once);
    }

    [Fact]
    public void Update_WithNullAccount_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _cachedRepository.Update(null));
    }

    [Fact]
    public void Delete_CallsUnderlyingRepository_AndRemovesFromCache()
    {
        // Arrange
        var existingAccount = _testBankAccounts[0];
        _mockRepository.Setup(r => r.Delete(existingAccount.Id)).Returns(true);

        // Act
        var result = _cachedRepository.Delete(existingAccount.Id);
        var accountAfterDelete = _cachedRepository.GetById(existingAccount.Id);

        // Assert
        Assert.True(result);
        Assert.Null(accountAfterDelete);
        _mockRepository.Verify(r => r.Delete(existingAccount.Id), Times.Once);
    }

    [Fact]
    public void Delete_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        _mockRepository.Setup(r => r.Delete(nonExistentId)).Returns(false);

        // Act
        var result = _cachedRepository.Delete(nonExistentId);

        // Assert
        Assert.False(result);
        _mockRepository.Verify(r => r.Delete(nonExistentId), Times.Once);
    }
}