using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;
using HSEBank.FinancialAccounting.Proxies;
using Moq;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Proxies;

public class CachedOperationRepositoryTests
{
    private readonly Mock<IOperationRepository> _mockInnerRepository;
    private readonly CachedOperationRepository _cachedRepository;
    private readonly Operation _testOperation;
    private readonly Guid _bankAccountId = Guid.NewGuid();
    private readonly Guid _categoryId = Guid.NewGuid();

    public CachedOperationRepositoryTests()
    {
        _mockInnerRepository = new Mock<IOperationRepository>();
        _cachedRepository = new CachedOperationRepository(_mockInnerRepository.Object);
        _testOperation = new Operation(
            Guid.NewGuid(),
            OperationType.Income,
            _bankAccountId,
            500m,
            DateTime.Now,
            _categoryId,
            "Test Operation");
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CachedOperationRepository(null));
    }

    [Fact]
    public void GetAll_FirstCall_CallsInnerRepository()
    {
        // Arrange
        var operations = new List<Operation> { _testOperation };
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(operations);

        // Act
        var result = _cachedRepository.GetAll();

        // Assert
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.Equal(operations, result);
    }

    [Fact]
    public void GetAll_SecondCall_UsesCache()
    {
        // Arrange
        var operations = new List<Operation> { _testOperation };
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(operations);

        // Act
        _cachedRepository.GetAll();
        var result = _cachedRepository.GetAll();

        // Assert
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.Equal(operations, result);
    }

    [Fact]
    public void GetById_FirstCall_InitializesCache()
    {
        // Arrange
        var operationId = _testOperation.Id;
        var operations = new List<Operation> { _testOperation };
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(operations);

        // Act
        var result = _cachedRepository.GetById(operationId);

        // Assert
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.Same(_testOperation, result);
    }

    [Fact]
    public void GetById_SecondCall_UsesCache()
    {
        // Arrange
        var operationId = _testOperation.Id;
        var operations = new List<Operation> { _testOperation };
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(operations);

        // Act
        _cachedRepository.GetById(operationId);
        var result = _cachedRepository.GetById(operationId);

        // Assert
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.Same(_testOperation, result);
    }

    [Fact]
    public void GetById_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var operations = new List<Operation> { _testOperation };
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(operations);

        // Act
        var result = _cachedRepository.GetById(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetByBankAccount_FirstCall_InitializesCache()
    {
        // Arrange
        var operations = new List<Operation> { _testOperation };
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(operations);

        // Act
        var result = _cachedRepository.GetByBankAccount(_bankAccountId);

        // Assert
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.Contains(_testOperation, result);
    }

    [Fact]
    public void GetByBankAccount_SecondCall_UsesCache()
    {
        // Arrange
        var operations = new List<Operation> { _testOperation };
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(operations);

        // Act
        _cachedRepository.GetByBankAccount(_bankAccountId);
        var result = _cachedRepository.GetByBankAccount(_bankAccountId);

        // Assert
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.Contains(_testOperation, result);
    }

    [Fact]
    public void GetByCategory_FirstCall_InitializesCache()
    {
        // Arrange
        var operations = new List<Operation> { _testOperation };
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(operations);

        // Act
        var result = _cachedRepository.GetByCategory(_categoryId);

        // Assert
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.Contains(_testOperation, result);
    }

    [Fact]
    public void GetByCategory_SecondCall_UsesCache()
    {
        // Arrange
        var operations = new List<Operation> { _testOperation };
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(operations);

        // Act
        _cachedRepository.GetByCategory(_categoryId);
        var result = _cachedRepository.GetByCategory(_categoryId);

        // Assert
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.Contains(_testOperation, result);
    }

    [Fact]
    public void GetByDateRange_FirstCall_InitializesCache()
    {
        // Arrange
        var operations = new List<Operation> { _testOperation };
        var startDate = DateTime.Now.AddDays(-7);
        var endDate = DateTime.Now.AddDays(1);
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(operations);

        // Act
        var result = _cachedRepository.GetByDateRange(startDate, endDate);

        // Assert
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.Contains(_testOperation, result);
    }

    [Fact]
    public void GetByDateRange_SecondCall_UsesCache()
    {
        // Arrange
        var operations = new List<Operation> { _testOperation };
        var startDate = DateTime.Now.AddDays(-7);
        var endDate = DateTime.Now.AddDays(1);
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(operations);

        // Act
        _cachedRepository.GetByDateRange(startDate, endDate);
        var result = _cachedRepository.GetByDateRange(startDate, endDate);

        // Assert
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.Contains(_testOperation, result);
    }

    [Fact]
    public void Add_CallsInnerRepository_AndInvalidatesCache()
    {
        // Arrange
        var initialOperations = new List<Operation>();
        var updatedOperations = new List<Operation> { _testOperation };
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(initialOperations);
        _mockInnerRepository.Setup(r => r.Add(It.IsAny<Operation>()));
            
            
        // Act
        _cachedRepository.GetAll();
        _cachedRepository.Add(_testOperation);
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(updatedOperations);
            
        // Assert
        _mockInnerRepository.Verify(r => r.Add(It.IsAny<Operation>()), Times.Once);
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(updatedOperations);
        var allOperations = _cachedRepository.GetAll();
        Assert.Equal(updatedOperations, allOperations);
    }

    [Fact]
    public void Update_CallsInnerRepository_AndInvalidatesCache()
    {
        // Arrange
        var operations = new List<Operation> { _testOperation };
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(operations);
        _mockInnerRepository.Setup(r => r.Update(It.IsAny<Operation>()));
            
        // Act
        _cachedRepository.GetAll();
        _cachedRepository.Update(_testOperation);
            
        // Assert
        _mockInnerRepository.Verify(r => r.Update(It.IsAny<Operation>()), Times.Once);
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(operations);
        var allOperations = _cachedRepository.GetAll();
        Assert.Equal(operations, allOperations);
    }

    [Fact]
    public void Delete_CallsInnerRepository_AndInvalidatesCache()
    {
        // Arrange
        var operationId = _testOperation.Id;
        var initialOperations = new List<Operation> { _testOperation };
        var updatedOperations = new List<Operation>();
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(initialOperations);
        _mockInnerRepository.Setup(r => r.Delete(It.IsAny<Guid>())).Returns(true);
            
        // Act
        _cachedRepository.GetAll();
        var result = _cachedRepository.Delete(operationId);
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(updatedOperations);
            
        // Assert
        _mockInnerRepository.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Once);
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.True(result);
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(updatedOperations);
        var allOperations = _cachedRepository.GetAll();
        Assert.Empty(allOperations);
    }
}