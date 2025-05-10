using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;
using HSEBank.FinancialAccounting.Repositories;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Repositories;

public class InMemoryOperationRepositoryTests
{
    private readonly InMemoryOperationRepository _repository;
    private readonly Operation _testOperation;
    private readonly Guid _operationId = Guid.NewGuid();
    private readonly Guid _bankAccountId = Guid.NewGuid();
    private readonly Guid _categoryId = Guid.NewGuid();
    private readonly DateTime _operationDate = new(2025, 3, 1);

    public InMemoryOperationRepositoryTests()
    {
        _repository = new InMemoryOperationRepository();
        _testOperation = new Operation(
            _operationId,
            OperationType.Income,
            _bankAccountId,
            100m,
            _operationDate,
            _categoryId,
            "Test Operation");
            
        _repository.Add(_testOperation);
    }

    [Fact]
    public void GetAll_ReturnsAllOperations()
    {
        // Arrange
        var additionalOperation = new Operation(
            Guid.NewGuid(),
            OperationType.Expense,
            _bankAccountId,
            50m,
            _operationDate.AddDays(1),
            _categoryId,
            "Additional Operation");
        _repository.Add(additionalOperation);

        // Act
        var result = _repository.GetAll();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(_testOperation, result);
        Assert.Contains(additionalOperation, result);
    }

    [Fact]
    public void GetById_WithExistingId_ReturnsOperation()
    {
        // Act
        var result = _repository.GetById(_operationId);

        // Assert
        Assert.Equal(_testOperation, result);
    }

    [Fact]
    public void GetById_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = _repository.GetById(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetByBankAccount_ReturnsOperationsForSpecificBankAccount()
    {
        // Arrange
        var anotherBankAccountId = Guid.NewGuid();
            
        var operationSameBankAccount = new Operation(
            Guid.NewGuid(),
            OperationType.Expense,
            _bankAccountId,
            50m,
            _operationDate.AddDays(1),
            _categoryId,
            "Same Bank Account Operation");
            
        var operationDifferentBankAccount = new Operation(
            Guid.NewGuid(),
            OperationType.Income,
            anotherBankAccountId,
            200m,
            _operationDate.AddDays(2),
            _categoryId,
            "Different Bank Account Operation");
            
        _repository.Add(operationSameBankAccount);
        _repository.Add(operationDifferentBankAccount);

        // Act
        var result = _repository.GetByBankAccount(_bankAccountId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(_testOperation, result);
        Assert.Contains(operationSameBankAccount, result);
        Assert.DoesNotContain(operationDifferentBankAccount, result);
    }

    [Fact]
    public void GetByCategory_ReturnsOperationsForSpecificCategory()
    {
        // Arrange
        var anotherCategoryId = Guid.NewGuid();
            
        var operationSameCategory = new Operation(
            Guid.NewGuid(),
            OperationType.Expense,
            _bankAccountId,
            50m,
            _operationDate.AddDays(1),
            _categoryId,
            "Same Category Operation");
            
        var operationDifferentCategory = new Operation(
            Guid.NewGuid(),
            OperationType.Income,
            _bankAccountId,
            200m,
            _operationDate.AddDays(2),
            anotherCategoryId,
            "Different Category Operation");
            
        _repository.Add(operationSameCategory);
        _repository.Add(operationDifferentCategory);

        // Act
        var result = _repository.GetByCategory(_categoryId);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(_testOperation, result);
        Assert.Contains(operationSameCategory, result);
        Assert.DoesNotContain(operationDifferentCategory, result);
    }

    [Fact]
    public void GetByType_ReturnsOperationsOfSpecificType()
    {
        // Arrange
        var incomeOperation = new Operation(
            Guid.NewGuid(),
            OperationType.Income,
            _bankAccountId,
            300m,
            _operationDate.AddDays(1),
            _categoryId,
            "Income Operation");
            
        var expenseOperation = new Operation(
            Guid.NewGuid(),
            OperationType.Expense,
            _bankAccountId,
            100m,
            _operationDate.AddDays(2),
            _categoryId,
            "Expense Operation");
            
        _repository.Add(incomeOperation);
        _repository.Add(expenseOperation);

        // Act
        var incomeResults = _repository.GetByType(OperationType.Income);
        var expenseResults = _repository.GetByType(OperationType.Expense);

        // Assert
        Assert.Equal(2, incomeResults.Count());
        Assert.Single(expenseResults);
            
        Assert.Contains(_testOperation, incomeResults);
        Assert.Contains(incomeOperation, incomeResults);
        Assert.Contains(expenseOperation, expenseResults);
    }

    [Fact]
    public void GetByDateRange_ReturnsOperationsWithinDateRange()
    {
        // Arrange
        var startDate = new DateTime(2025, 2, 28);
        var endDate = new DateTime(2025, 3, 3);
            
        var operationBefore = new Operation(
            Guid.NewGuid(),
            OperationType.Income,
            _bankAccountId,
            100m,
            startDate.AddDays(-1),
            _categoryId,
            "Operation Before Range");
            
        var operationInRange1 = new Operation(
            Guid.NewGuid(),
            OperationType.Income,
            _bankAccountId,
            200m,
            startDate,
            _categoryId,
            "Operation In Range 1");
            
        var operationInRange2 = new Operation(
            Guid.NewGuid(),
            OperationType.Expense,
            _bankAccountId,
            50m,
            endDate,
            _categoryId,
            "Operation In Range 2");
            
        var operationAfter = new Operation(
            Guid.NewGuid(),
            OperationType.Expense,
            _bankAccountId,
            75m,
            endDate.AddDays(1),
            _categoryId,
            "Operation After Range");
            
        _repository.Add(operationBefore);
        _repository.Add(operationInRange1);
        _repository.Add(operationInRange2);
        _repository.Add(operationAfter);

        // Act
        var result = _repository.GetByDateRange(startDate, endDate);

        // Assert
        Assert.Equal(3, result.Count());
        Assert.Contains(_testOperation, result);
        Assert.Contains(operationInRange1, result);
        Assert.Contains(operationInRange2, result);
        Assert.DoesNotContain(operationBefore, result);
        Assert.DoesNotContain(operationAfter, result);
    }

    [Fact]
    public void GetByBankAccountAndDateRange_ReturnsOperationsForBankAccountWithinDateRange()
    {
        // Arrange
        var startDate = new DateTime(2025, 2, 28);
        var endDate = new DateTime(2025, 3, 3);
        var anotherBankAccountId = Guid.NewGuid();
            
        var operationBefore = new Operation(
            Guid.NewGuid(),
            OperationType.Income,
            _bankAccountId,
            100m,
            startDate.AddDays(-1),
            _categoryId,
            "Operation Before Range");
            
        var operationInRange1 = new Operation(
            Guid.NewGuid(),
            OperationType.Income,
            _bankAccountId,
            200m,
            startDate,
            _categoryId,
            "Operation In Range 1");
            
        var operationInRangeDifferentBankAccount = new Operation(
            Guid.NewGuid(),
            OperationType.Income,
            anotherBankAccountId,
            300m,
            startDate.AddDays(1),
            _categoryId,
            "Operation In Range Different Bank Account");
            
        var operationAfter = new Operation(
            Guid.NewGuid(),
            OperationType.Expense,
            _bankAccountId,
            75m,
            endDate.AddDays(1),
            _categoryId,
            "Operation After Range");
            
        _repository.Add(operationBefore);
        _repository.Add(operationInRange1);
        _repository.Add(operationInRangeDifferentBankAccount);
        _repository.Add(operationAfter);

        // Act
        var result = _repository.GetByBankAccountAndDateRange(_bankAccountId, startDate, endDate);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(_testOperation, result);
        Assert.Contains(operationInRange1, result);
        Assert.DoesNotContain(operationInRangeDifferentBankAccount, result);
        Assert.DoesNotContain(operationBefore, result);
        Assert.DoesNotContain(operationAfter, result);
    }

    [Fact]
    public void Add_WithValidOperation_AddsToRepository()
    {
        // Arrange
        var newOperation = new Operation(
            Guid.NewGuid(),
            OperationType.Expense,
            _bankAccountId,
            50m,
            DateTime.Now,
            _categoryId,
            "New Operation");

        // Act
        _repository.Add(newOperation);
        var retrievedOperation = _repository.GetById(newOperation.Id);

        // Assert
        Assert.NotNull(retrievedOperation);
        Assert.Equal(newOperation, retrievedOperation);
    }

    [Fact]
    public void Add_WithNullOperation_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _repository.Add(null));
    }

    [Fact]
    public void Add_WithDuplicateId_ThrowsArgumentException()
    {
        // Arrange
        var duplicateOperation = new Operation(
            _operationId,
            OperationType.Expense,
            _bankAccountId,
            50m,
            DateTime.Now,
            _categoryId,
            "Duplicate ID");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _repository.Add(duplicateOperation));
    }

    [Fact]
    public void Update_WithExistingOperation_UpdatesOperation()
    {
        // Arrange
        var updatedOperation = new Operation(
            _operationId,
            OperationType.Expense,
            _bankAccountId,
            200m,
            _operationDate.AddDays(1),
            _categoryId,
            "Updated Operation");

        // Act
        _repository.Update(updatedOperation);
        var retrievedOperation = _repository.GetById(_operationId);

        // Assert
        Assert.Equal(updatedOperation, retrievedOperation);
        Assert.Equal(OperationType.Expense, retrievedOperation.Type);
        Assert.Equal(200m, retrievedOperation.Amount);
        Assert.Equal("Updated Operation", retrievedOperation.Description);
    }

    [Fact]
    public void Update_WithNullOperation_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _repository.Update(null));
    }

    [Fact]
    public void Update_WithNonExistentId_ThrowsArgumentException()
    {
        // Arrange
        var nonExistentOperation = new Operation(
            Guid.NewGuid(),
            OperationType.Income,
            _bankAccountId,
            100m,
            DateTime.Now,
            _categoryId,
            "Non-existent Operation");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _repository.Update(nonExistentOperation));
    }

    [Fact]
    public void Delete_WithExistingId_RemovesOperation()
    {
        // Act
        var result = _repository.Delete(_operationId);
        var retrievedOperation = _repository.GetById(_operationId);

        // Assert
        Assert.True(result);
        Assert.Null(retrievedOperation);
    }

    [Fact]
    public void Delete_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = _repository.Delete(nonExistentId);

        // Assert
        Assert.False(result);
    }
}