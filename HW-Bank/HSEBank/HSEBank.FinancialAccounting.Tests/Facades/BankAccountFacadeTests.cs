using HSEBank.FinancialAccounting.Facades;
using HSEBank.FinancialAccounting.Interfaces.Factories;
using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Models;
using Moq;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Facades;

public class BankAccountFacadeTests
{
    private readonly Mock<IBankAccountRepository> _mockRepository;
    private readonly Mock<IBankAccountFactory> _mockFactory;
    private readonly BankAccountFacade _facade;
    private readonly BankAccount _testBankAccount;

    public BankAccountFacadeTests()
    {
        _mockRepository = new Mock<IBankAccountRepository>();
        _mockFactory = new Mock<IBankAccountFactory>();
        _facade = new BankAccountFacade(_mockRepository.Object, _mockFactory.Object);
            
        _testBankAccount = new BankAccount(Guid.NewGuid(), "Test Account", 1000m);
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BankAccountFacade(null, _mockFactory.Object));
    }

    [Fact]
    public void Constructor_WithNullFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BankAccountFacade(_mockRepository.Object, null));
    }

    [Fact]
    public void GetAllBankAccounts_CallsRepositoryGetAll()
    {
        // Arrange
        var bankAccounts = new[] { _testBankAccount };
        _mockRepository.Setup(r => r.GetAll()).Returns(bankAccounts);

        // Act
        var result = _facade.GetAllBankAccounts();

        // Assert
        Assert.Same(bankAccounts, result);
        _mockRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void GetBankAccountById_CallsRepositoryGetById()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetById(_testBankAccount.Id)).Returns(_testBankAccount);

        // Act
        var result = _facade.GetBankAccountById(_testBankAccount.Id);

        // Assert
        Assert.Same(_testBankAccount, result);
        _mockRepository.Verify(r => r.GetById(_testBankAccount.Id), Times.Once);
    }

    [Fact]
    public void GetBankAccountByName_CallsRepositoryGetByName()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByName(_testBankAccount.Name)).Returns(_testBankAccount);

        // Act
        var result = _facade.GetBankAccountByName(_testBankAccount.Name);

        // Assert
        Assert.Same(_testBankAccount, result);
        _mockRepository.Verify(r => r.GetByName(_testBankAccount.Name), Times.Once);
    }

    [Fact]
    public void CreateBankAccount_CallsFactoryCreate_AndRepositoryAdd()
    {
        // Arrange
        string name = "New Account";
        decimal initialBalance = 2000m;
            
        _mockFactory.Setup(f => f.Create(name, initialBalance)).Returns(_testBankAccount);

        // Act
        var result = _facade.CreateBankAccount(name, initialBalance);

        // Assert
        Assert.Same(_testBankAccount, result);
        _mockFactory.Verify(f => f.Create(name, initialBalance), Times.Once);
        _mockRepository.Verify(r => r.Add(_testBankAccount), Times.Once);
    }

    [Fact]
    public void UpdateBankAccountName_CallsRepositoryGetById_AndUpdate()
    {
        // Arrange
        string newName = "Updated Account";
            
        _mockRepository.Setup(r => r.GetById(_testBankAccount.Id)).Returns(_testBankAccount);

        // Act
        var result = _facade.UpdateBankAccountName(_testBankAccount.Id, newName);

        // Assert
        Assert.Same(_testBankAccount, result);
        Assert.Equal(newName, _testBankAccount.Name);
        _mockRepository.Verify(r => r.GetById(_testBankAccount.Id), Times.Once);
        _mockRepository.Verify(r => r.Update(_testBankAccount), Times.Once);
    }

    [Fact]
    public void UpdateBankAccountName_WhenBankAccountNotFound_ThrowsArgumentException()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        string newName = "Updated Account";
            
        _mockRepository.Setup(r => r.GetById(id)).Returns((BankAccount)null);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _facade.UpdateBankAccountName(id, newName));
        Assert.Contains(id.ToString(), exception.Message);
    }

    [Fact]
    public void UpdateBankAccountBalance_CallsRepositoryGetById_AndUpdate()
    {
        // Arrange
        decimal newBalance = 2000m;
            
        _mockRepository.Setup(r => r.GetById(_testBankAccount.Id)).Returns(_testBankAccount);

        // Act
        var result = _facade.UpdateBankAccountBalance(_testBankAccount.Id, newBalance);

        // Assert
        Assert.Same(_testBankAccount, result);
        Assert.Equal(newBalance, _testBankAccount.Balance);
        _mockRepository.Verify(r => r.GetById(_testBankAccount.Id), Times.Once);
        _mockRepository.Verify(r => r.Update(_testBankAccount), Times.Once);
    }

    [Fact]
    public void UpdateBankAccountBalance_WhenBankAccountNotFound_ThrowsArgumentException()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        decimal newBalance = 2000m;
            
        _mockRepository.Setup(r => r.GetById(id)).Returns((BankAccount)null);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _facade.UpdateBankAccountBalance(id, newBalance));
        Assert.Contains(id.ToString(), exception.Message);
    }

    [Fact]
    public void DeleteBankAccount_CallsRepositoryDelete()
    {
        // Arrange
        _mockRepository.Setup(r => r.Delete(_testBankAccount.Id)).Returns(true);

        // Act
        var result = _facade.DeleteBankAccount(_testBankAccount.Id);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.Delete(_testBankAccount.Id), Times.Once);
    }
}