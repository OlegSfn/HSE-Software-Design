using HSEBank.FinancialAccounting.Commands;
using HSEBank.FinancialAccounting.Interfaces.Facades;
using HSEBank.FinancialAccounting.Models;
using Moq;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Commands;

public class CreateBankAccountCommandTests
{
    private readonly Mock<IBankAccountFacade> _mockFacade;
    private readonly CreateBankAccountCommand _command;
    private readonly string _name;
    private readonly decimal _initialBalance;
    private readonly BankAccount _createdBankAccount;

    public CreateBankAccountCommandTests()
    {
        _mockFacade = new Mock<IBankAccountFacade>();
        _name = "Test Account";
        _initialBalance = 1000m;
        _command = new CreateBankAccountCommand(_mockFacade.Object, _name, _initialBalance);
        _createdBankAccount = new BankAccount(Guid.NewGuid(), _name, _initialBalance);
    }

    [Fact]
    public void Constructor_WithNullFacade_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CreateBankAccountCommand(null, _name, _initialBalance));
    }

    [Fact]
    public void Constructor_WithNullName_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CreateBankAccountCommand(_mockFacade.Object, null, _initialBalance));
    }

    [Fact]
    public void Execute_CallsFacadeCreateBankAccount()
    {
        // Arrange
        _mockFacade.Setup(f => f.CreateBankAccount(_name, _initialBalance)).Returns(_createdBankAccount);

        // Act
        _command.Execute();

        // Assert
        _mockFacade.Verify(f => f.CreateBankAccount(_name, _initialBalance), Times.Once);
        Assert.Same(_createdBankAccount, _command.CreatedBankAccount);
    }

    [Fact]
    public void Undo_WhenBankAccountCreated_CallsFacadeDeleteBankAccount()
    {
        // Arrange
        _mockFacade.Setup(f => f.CreateBankAccount(_name, _initialBalance)).Returns(_createdBankAccount);
        _mockFacade.Setup(f => f.DeleteBankAccount(_createdBankAccount.Id)).Returns(true);
        _command.Execute();

        // Act
        _command.Undo();

        // Assert
        _mockFacade.Verify(f => f.DeleteBankAccount(_createdBankAccount.Id), Times.Once);
    }

    [Fact]
    public void Undo_WhenBankAccountIsNotCreated_DoesNothing()
    {
        // Act
        _command.Undo();

        // Assert
        _mockFacade.Verify(f => f.DeleteBankAccount(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void CreatedBankAccount_BeforeExecute_ReturnsNull()
    {
        // Act & Assert
        Assert.Null(_command.CreatedBankAccount);
    }

    [Fact]
    public void CreatedBankAccount_AfterExecute_ReturnsBankAccount()
    {
        // Arrange
        _mockFacade.Setup(f => f.CreateBankAccount(_name, _initialBalance)).Returns(_createdBankAccount);

        // Act
        _command.Execute();

        // Assert
        Assert.Same(_createdBankAccount, _command.CreatedBankAccount);
    }

    [Fact]
    public void CreatedBankAccount_AfterUndo_ReturnsNull()
    {
        // Arrange
        _mockFacade.Setup(f => f.CreateBankAccount(_name, _initialBalance)).Returns(_createdBankAccount);
        _mockFacade.Setup(f => f.DeleteBankAccount(_createdBankAccount.Id)).Returns(true);
        _command.Execute();

        // Act
        _command.Undo();

        // Assert
        Assert.Null(_command.CreatedBankAccount);
    }
}