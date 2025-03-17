using HSEBank.FinancialAccounting.Interfaces.Facades;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;
using HSEBank.FinancialAccounting.TemplateMethod;
using Moq;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.TemplateMethod;

public class JsonDataImporterTests : IDisposable
{
    private readonly Mock<IBankAccountFacade> _mockBankAccountFacade;
    private readonly Mock<ICategoryFacade> _mockCategoryFacade;
    private readonly Mock<IOperationFacade> _mockOperationFacade;
    private readonly JsonDataImporter _jsonDataImporter;
    private readonly string _tempFilePath;

    public JsonDataImporterTests()
    {
        _mockBankAccountFacade = new Mock<IBankAccountFacade>();
        _mockCategoryFacade = new Mock<ICategoryFacade>();
        _mockOperationFacade = new Mock<IOperationFacade>();

        _jsonDataImporter = new JsonDataImporter(
            _mockBankAccountFacade.Object,
            _mockCategoryFacade.Object,
            _mockOperationFacade.Object);

        _tempFilePath = Path.GetTempFileName();
    }

    public void Dispose()
    {
        if (File.Exists(_tempFilePath))
        {
            File.Delete(_tempFilePath);
        }
    }

    [Fact]
    public void Constructor_WithNullBankAccountFacade_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new JsonDataImporter(
            null,
            _mockCategoryFacade.Object,
            _mockOperationFacade.Object));
    }

    [Fact]
    public void Constructor_WithNullCategoryFacade_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new JsonDataImporter(
            _mockBankAccountFacade.Object,
            null,
            _mockOperationFacade.Object));
    }

    [Fact]
    public void Constructor_WithNullOperationFacade_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new JsonDataImporter(
            _mockBankAccountFacade.Object,
            _mockCategoryFacade.Object,
            null));
    }

    [Fact]
    public void ImportData_WithValidJsonFile_ReturnsTrue()
    {
        // Arrange
        var validJson = @"{
                ""bankAccounts"": [
                    {
                        ""name"": ""Test Account"",
                        ""balance"": 1000
                    }
                ],
                ""categories"": [
                    {
                        ""type"": 0,
                        ""name"": ""Test Income Category""
                    },
                    {
                        ""type"": 1,
                        ""name"": ""Test Expense Category""
                    }
                ],
                ""operations"": [
                    {
                        ""type"": 0,
                        ""bankAccountName"": ""Test Account"",
                        ""amount"": 500,
                        ""date"": ""2023-01-01T00:00:00"",
                        ""categoryName"": ""Test Income Category"",
                        ""description"": ""Test Income Operation""
                    },
                    {
                        ""type"": 1,
                        ""bankAccountName"": ""Test Account"",
                        ""amount"": 200,
                        ""date"": ""2023-01-02T00:00:00"",
                        ""categoryName"": ""Test Expense Category"",
                        ""description"": ""Test Expense Operation""
                    }
                ]
            }";
            
        File.WriteAllText(_tempFilePath, validJson);

        var bankAccount = new BankAccount(Guid.NewGuid(), "Test Account", 1000m);
        var incomeCategory = new Category(Guid.NewGuid(), CategoryType.Income, "Test Income Category");
        var expenseCategory = new Category(Guid.NewGuid(), CategoryType.Expense, "Test Expense Category");

        _mockBankAccountFacade.Setup(f => f.GetBankAccountByName("Test Account")).Returns(bankAccount);
        _mockCategoryFacade.Setup(f => f.GetCategoryByName("Test Income Category")).Returns(incomeCategory);
        _mockCategoryFacade.Setup(f => f.GetCategoryByName("Test Expense Category")).Returns(expenseCategory);

        // Act
        var result = _jsonDataImporter.ImportData(_tempFilePath);

        // Assert
        Assert.True(result);

        _mockBankAccountFacade.Verify(f => f.CreateBankAccount("Test Account", 1000m), Times.Once);
        _mockCategoryFacade.Verify(f => f.CreateCategory(CategoryType.Income, "Test Income Category"), Times.Once);
        _mockCategoryFacade.Verify(f => f.CreateCategory(CategoryType.Expense, "Test Expense Category"), Times.Once);
        _mockOperationFacade.Verify(f => f.CreateOperation(
            OperationType.Income, 
            bankAccount.Id, 
            500m, 
            new DateTime(2023, 1, 1), 
            incomeCategory.Id, 
            "Test Income Operation"), Times.Once);
        _mockOperationFacade.Verify(f => f.CreateOperation(
            OperationType.Expense, 
            bankAccount.Id, 
            200m, 
            new DateTime(2023, 1, 2), 
            expenseCategory.Id, 
            "Test Expense Operation"), Times.Once);
    }

    [Fact]
    public void ImportData_WithInvalidJsonFile_ReturnsFalse()
    {
        // Arrange
        var invalidJson = "This is not a valid JSON";
        File.WriteAllText(_tempFilePath, invalidJson);

        // Act
        var result = _jsonDataImporter.ImportData(_tempFilePath);

        // Assert
        Assert.False(result);
        _mockBankAccountFacade.Verify(f => f.CreateBankAccount(It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
        _mockCategoryFacade.Verify(f => f.CreateCategory(It.IsAny<CategoryType>(), It.IsAny<string>()), Times.Never);
        _mockOperationFacade.Verify(f => f.CreateOperation(
            It.IsAny<OperationType>(),
            It.IsAny<Guid>(),
            It.IsAny<decimal>(),
            It.IsAny<DateTime>(),
            It.IsAny<Guid>(),
            It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void ImportData_WithMissingRequiredData_ReturnsFalse()
    {
        // Arrange
        var invalidJson = @"{
                ""bankAccounts"": [],
                ""categories"": [],
                ""operations"": [
                    {
                        ""type"": 0,
                        ""bankAccountName"": ""Non-existent Account"",
                        ""amount"": 500,
                        ""date"": ""2023-01-01T00:00:00"",
                        ""categoryName"": ""Non-existent Category"",
                        ""description"": ""This will fail""
                    }
                ]
            }";
            
        File.WriteAllText(_tempFilePath, invalidJson);

        // Act
        var result = _jsonDataImporter.ImportData(_tempFilePath);

        // Assert
        Assert.True(result);
            
        _mockBankAccountFacade.Verify(f => f.CreateBankAccount(It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
        _mockCategoryFacade.Verify(f => f.CreateCategory(It.IsAny<CategoryType>(), It.IsAny<string>()), Times.Never);
            
        _mockOperationFacade.Verify(f => f.CreateOperation(
            It.IsAny<OperationType>(),
            It.IsAny<Guid>(),
            It.IsAny<decimal>(),
            It.IsAny<DateTime>(),
            It.IsAny<Guid>(),
            It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void ImportData_WithEmptyJsonFile_ReturnsSuccessWithNoActions()
    {
        // Arrange
        File.WriteAllText(_tempFilePath, "{}");

        // Act
        var result = _jsonDataImporter.ImportData(_tempFilePath);

        // Assert
        Assert.True(result);
            
        _mockBankAccountFacade.Verify(f => f.CreateBankAccount(It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
        _mockCategoryFacade.Verify(f => f.CreateCategory(It.IsAny<CategoryType>(), It.IsAny<string>()), Times.Never);
        _mockOperationFacade.Verify(f => f.CreateOperation(
            It.IsAny<OperationType>(),
            It.IsAny<Guid>(),
            It.IsAny<decimal>(),
            It.IsAny<DateTime>(),
            It.IsAny<Guid>(),
            It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void ImportData_WithCorruptedJsonFile_ReturnsFalse()
    {
        // Arrange
        var corruptedJson = @"{
                ""bankAccounts"": [
                    {
                        ""name"": ""Test Account"",
                        ""balance"": 1000
                    }
                ],
                ""categories"": [
                    {
                        ""type"": 0,
                        ""name"": ""Test Category""
                    }
                ],
                ""operations"": [
                    {
                        ""type"": 0,
                        ""bankAccountName"":";
            
        File.WriteAllText(_tempFilePath, corruptedJson);

        // Act
        var result = _jsonDataImporter.ImportData(_tempFilePath);

        // Assert
        Assert.False(result);
        _mockBankAccountFacade.Verify(f => f.CreateBankAccount(It.IsAny<string>(), It.IsAny<decimal>()), Times.Never);
        _mockCategoryFacade.Verify(f => f.CreateCategory(It.IsAny<CategoryType>(), It.IsAny<string>()), Times.Never);
        _mockOperationFacade.Verify(f => f.CreateOperation(
            It.IsAny<OperationType>(),
            It.IsAny<Guid>(),
            It.IsAny<decimal>(),
            It.IsAny<DateTime>(),
            It.IsAny<Guid>(),
            It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void ImportData_WithMissingBankAccount_SkipsOperation()
    {
        // Arrange
        var json = @"{
                ""bankAccounts"": [
                    {
                        ""name"": ""Test Account"",
                        ""balance"": 1000
                    }
                ],
                ""categories"": [
                    {
                        ""type"": 0,
                        ""name"": ""Test Category""
                    }
                ],
                ""operations"": [
                    {
                        ""type"": 0,
                        ""bankAccountName"": ""Missing Account"",
                        ""amount"": 500,
                        ""date"": ""2023-01-01T00:00:00"",
                        ""categoryName"": ""Test Category"",
                        ""description"": ""Test Operation""
                    }
                ]
            }";
            
        File.WriteAllText(_tempFilePath, json);

        var category = new Category(Guid.NewGuid(), CategoryType.Income, "Test Category");
        _mockCategoryFacade.Setup(f => f.GetCategoryByName("Test Category")).Returns(category);
        _mockBankAccountFacade.Setup(f => f.GetBankAccountByName("Missing Account")).Returns((BankAccount)null);

        // Act
        var result = _jsonDataImporter.ImportData(_tempFilePath);

        // Assert
        Assert.True(result);
        _mockBankAccountFacade.Verify(f => f.CreateBankAccount("Test Account", 1000m), Times.Once);
        _mockCategoryFacade.Verify(f => f.CreateCategory(CategoryType.Income, "Test Category"), Times.Once);
        _mockOperationFacade.Verify(f => f.CreateOperation(
            It.IsAny<OperationType>(),
            It.IsAny<Guid>(),
            It.IsAny<decimal>(),
            It.IsAny<DateTime>(),
            It.IsAny<Guid>(),
            It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void ImportData_WithMissingCategory_SkipsOperation()
    {
        // Arrange
        var json = @"{
                ""bankAccounts"": [
                    {
                        ""name"": ""Test Account"",
                        ""balance"": 1000
                    }
                ],
                ""categories"": [
                    {
                        ""type"": 0,
                        ""name"": ""Test Category""
                    }
                ],
                ""operations"": [
                    {
                        ""type"": 0,
                        ""bankAccountName"": ""Test Account"",
                        ""amount"": 500,
                        ""date"": ""2023-01-01T00:00:00"",
                        ""categoryName"": ""Missing Category"",
                        ""description"": ""Test Operation""
                    }
                ]
            }";
            
        File.WriteAllText(_tempFilePath, json);

        var bankAccount = new BankAccount(Guid.NewGuid(), "Test Account", 1000m);
        _mockBankAccountFacade.Setup(f => f.GetBankAccountByName("Test Account")).Returns(bankAccount);
        _mockCategoryFacade.Setup(f => f.GetCategoryByName("Missing Category")).Returns((Category)null);

        // Act
        var result = _jsonDataImporter.ImportData(_tempFilePath);

        // Assert
        Assert.True(result);
        _mockBankAccountFacade.Verify(f => f.CreateBankAccount("Test Account", 1000m), Times.Once);
        _mockCategoryFacade.Verify(f => f.CreateCategory(CategoryType.Income, "Test Category"), Times.Once);
        _mockOperationFacade.Verify(f => f.CreateOperation(
            It.IsAny<OperationType>(),
            It.IsAny<Guid>(),
            It.IsAny<decimal>(),
            It.IsAny<DateTime>(),
            It.IsAny<Guid>(),
            It.IsAny<string>()), Times.Never);
    }
}