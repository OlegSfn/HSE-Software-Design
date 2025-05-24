using HSEBank.FinancialAccounting.Commands;
using HSEBank.FinancialAccounting.Facades;
using HSEBank.FinancialAccounting.Factories;
using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;
using Moq;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Commands;

public class CreateOperationCommandTests
{
    private readonly Mock<IOperationRepository> _mockOperationRepository;
    private readonly Mock<IBankAccountRepository> _mockBankAccountRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly OperationFacade _facade;
    private readonly CreateOperationCommand _command;
    private readonly OperationType _type;
    private readonly Guid _bankAccountId;
    private readonly decimal _amount;
    private readonly DateTime _date;
    private readonly Guid _categoryId;
    private readonly string _description;
    private readonly BankAccount _bankAccount;

    public CreateOperationCommandTests()
    {
        _mockOperationRepository = new Mock<IOperationRepository>();
        _mockBankAccountRepository = new Mock<IBankAccountRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        var operationFactory = new OperationFactory();
        _facade = new OperationFacade(
            _mockOperationRepository.Object,
            _mockBankAccountRepository.Object,
            _mockCategoryRepository.Object,
            operationFactory);

        _type = OperationType.Income;
        _bankAccountId = Guid.NewGuid();
        _amount = 1000m;
        _date = DateTime.Now;
        _categoryId = Guid.NewGuid();
        _description = "Test operation";

        _bankAccount = new BankAccount(_bankAccountId, "Test Account", 5000m);
        var category = new Category(_categoryId, CategoryType.Income, "Test Category");

        _mockBankAccountRepository.Setup(r => r.GetById(_bankAccountId)).Returns(_bankAccount);
        _mockCategoryRepository.Setup(r => r.GetById(_categoryId)).Returns(category);

        _command = new CreateOperationCommand(
            _facade,
            _type,
            _bankAccountId,
            _amount,
            _date,
            _categoryId,
            _description);
    }

    [Fact]
    public void Constructor_WithNullFacade_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CreateOperationCommand(
            null,
            _type,
            _bankAccountId,
            _amount,
            _date,
            _categoryId,
            _description));
    }

    [Fact]
    public void Execute_CallsRepositoryAdd()
    {
        // Act
        _command.Execute();

        // Assert
        _mockOperationRepository.Verify(r => r.Add(It.IsAny<Operation>()), Times.Once);
    }

    [Fact]
    public void Execute_UpdatesBankAccountBalance()
    {
        // Arrange
        decimal initialBalance = _bankAccount.Balance;

        // Act
        _command.Execute();

        // Assert
        _mockBankAccountRepository.Verify(r => r.Update(_bankAccount), Times.Once);
        Assert.Equal(initialBalance + _amount, _bankAccount.Balance);
    }

    [Fact]
    public void Execute_WithExpenseType_DecreasesBalance()
    {
        // Arrange
        var expenseCategory = new Category(Guid.NewGuid(), CategoryType.Expense, "Expense Category");
        _mockCategoryRepository.Setup(r => r.GetById(expenseCategory.Id)).Returns(expenseCategory);

        decimal initialBalance = _bankAccount.Balance;
        var expenseCommand = new CreateOperationCommand(
            _facade,
            OperationType.Expense,
            _bankAccountId,
            _amount,
            _date,
            expenseCategory.Id,
            _description);

        // Act
        expenseCommand.Execute();

        // Assert
        _mockBankAccountRepository.Verify(r => r.Update(_bankAccount), Times.Once);
        Assert.Equal(initialBalance - _amount, _bankAccount.Balance);
    }

    [Fact]
    public void Undo_WhenOperationCreated_CallsRepositoryDelete()
    {
        // Arrange
        _command.Execute();
        var createdOperation = _command.CreatedOperation;
        _mockOperationRepository.Setup(r => r.GetById(createdOperation.Id)).Returns(createdOperation);
        _mockOperationRepository.Setup(r => r.Delete(createdOperation.Id)).Returns(true);

        // Act
        _command.Undo();

        // Assert
        _mockOperationRepository.Verify(r => r.Delete(createdOperation.Id), Times.Once);
    }

    [Fact]
    public void Undo_WhenOperationCreated_RevertsBankAccountBalance()
    {
        // Arrange
        decimal initialBalance = _bankAccount.Balance;
        _command.Execute();
        var createdOperation = _command.CreatedOperation;
        _mockOperationRepository.Setup(r => r.GetById(createdOperation.Id)).Returns(createdOperation);
        _mockOperationRepository.Setup(r => r.Delete(createdOperation.Id)).Returns(true);

        // Act
        _command.Undo();

        // Assert
        Assert.Equal(initialBalance, _bankAccount.Balance);
    }

    [Fact]
    public void Undo_WhenOperationNotCreated_DoesNothing()
    {
        // Act
        _command.Undo();

        // Assert
        _mockOperationRepository.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
        _mockBankAccountRepository.Verify(r => r.Update(It.IsAny<BankAccount>()), Times.Never);
    }

    [Fact]
    public void CreatedOperation_BeforeExecute_ReturnsNull()
    {
        // Act & Assert
        Assert.Null(_command.CreatedOperation);
    }

    [Fact]
    public void CreatedOperation_AfterExecute_ReturnsOperation()
    {
        // Act
        _command.Execute();

        // Assert
        Assert.NotNull(_command.CreatedOperation);
        Assert.Equal(_type, _command.CreatedOperation.Type);
        Assert.Equal(_bankAccountId, _command.CreatedOperation.BankAccountId);
        Assert.Equal(_amount, _command.CreatedOperation.Amount);
        Assert.Equal(_date, _command.CreatedOperation.Date);
        Assert.Equal(_categoryId, _command.CreatedOperation.CategoryId);
        Assert.Equal(_description, _command.CreatedOperation.Description);
    }

    [Fact]
    public void CreatedOperation_AfterUndo_ReturnsNull()
    {
        // Arrange
        _command.Execute();
        _mockOperationRepository.Setup(r => r.Delete(It.IsAny<Guid>())).Returns(true);

        // Act
        _command.Undo();

        // Assert
        Assert.Null(_command.CreatedOperation);
    }
}