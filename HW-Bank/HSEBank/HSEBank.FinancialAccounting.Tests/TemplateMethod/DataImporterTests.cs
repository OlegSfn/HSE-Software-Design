using HSEBank.FinancialAccounting.Interfaces.Facades;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;
using HSEBank.FinancialAccounting.TemplateMethod;
using Moq;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.TemplateMethod;

public class DataImporterTests
{
    private readonly Mock<IBankAccountFacade> _mockBankAccountFacade;
    private readonly Mock<ICategoryFacade> _mockCategoryFacade;
    private readonly Mock<IOperationFacade> _mockOperationFacade;
    private readonly TestDataImporter _dataImporter;
    private readonly BankAccount _testBankAccount;
    private readonly Category _testCategory;

    public DataImporterTests()
    {
        _mockBankAccountFacade = new Mock<IBankAccountFacade>();
        _mockCategoryFacade = new Mock<ICategoryFacade>();
        _mockOperationFacade = new Mock<IOperationFacade>();

        _dataImporter = new TestDataImporter(
            _mockBankAccountFacade.Object,
            _mockCategoryFacade.Object,
            _mockOperationFacade.Object);

        _testBankAccount = new BankAccount(Guid.NewGuid(), "Test Account", 1000m);
        _testCategory = new Category(Guid.NewGuid(), CategoryType.Income, "Test Category");
    }

    [Fact]
    public void Constructor_WithNullBankAccountFacade_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TestDataImporter(
            null,
            _mockCategoryFacade.Object,
            _mockOperationFacade.Object));
    }

    [Fact]
    public void Constructor_WithNullCategoryFacade_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TestDataImporter(
            _mockBankAccountFacade.Object,
            null,
            _mockOperationFacade.Object));
    }

    [Fact]
    public void Constructor_WithNullOperationFacade_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TestDataImporter(
            _mockBankAccountFacade.Object,
            _mockCategoryFacade.Object,
            null));
    }

    [Fact]
    public void ImportData_CreatesEntities()
    {
        // Arrange
        string filePath = "test.json";
        string bankAccountName = "Test Account";
        string categoryName = "Test Category";
        decimal balance = 1000m;
        decimal amount = 500m;
        string description = "Test Operation";

        _mockBankAccountFacade.Setup(f => f.GetBankAccountByName(bankAccountName)).Returns(_testBankAccount);
        _mockCategoryFacade.Setup(f => f.GetCategoryByName(categoryName)).Returns(_testCategory);

        // Act
        bool result = _dataImporter.ImportData(filePath);

        // Assert
        Assert.True(result);
        _mockBankAccountFacade.Verify(f => f.CreateBankAccount(bankAccountName, balance), Times.Once);
        _mockCategoryFacade.Verify(f => f.CreateCategory(CategoryType.Income, categoryName), Times.Once);
        _mockOperationFacade.Verify(f => f.CreateOperation(
            OperationType.Income,
            _testBankAccount.Id,
            amount,
            It.IsAny<DateTime>(),
            _testCategory.Id,
            description), Times.Once);
    }

    [Fact]
    public void ImportData_WithException_ReturnsFalse()
    {
        // Arrange
        string filePath = "test.json";
        _dataImporter.ShouldThrowOnRead = true;

        // Act
        bool result = _dataImporter.ImportData(filePath);

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
        string filePath = "test.json";
        string bankAccountName = "Test Account";
        string categoryName = "Test Category";

        _mockBankAccountFacade.Setup(f => f.GetBankAccountByName(bankAccountName)).Returns((BankAccount)null);
        _mockCategoryFacade.Setup(f => f.GetCategoryByName(categoryName)).Returns(_testCategory);

        // Act
        bool result = _dataImporter.ImportData(filePath);

        // Assert
        Assert.True(result);
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
        string filePath = "test.json";
        string bankAccountName = "Test Account";
        string categoryName = "Test Category";

        _mockBankAccountFacade.Setup(f => f.GetBankAccountByName(bankAccountName)).Returns(_testBankAccount);
        _mockCategoryFacade.Setup(f => f.GetCategoryByName(categoryName)).Returns((Category)null);

        // Act
        bool result = _dataImporter.ImportData(filePath);

        // Assert
        Assert.True(result);
        _mockOperationFacade.Verify(f => f.CreateOperation(
            It.IsAny<OperationType>(),
            It.IsAny<Guid>(),
            It.IsAny<decimal>(),
            It.IsAny<DateTime>(),
            It.IsAny<Guid>(),
            It.IsAny<string>()), Times.Never);
    }

    private class TestDataImporter : DataImporter
    {
        public bool ShouldThrowOnRead { get; set; }

        public TestDataImporter(IBankAccountFacade bankAccountFacade, ICategoryFacade categoryFacade, IOperationFacade operationFacade)
            : base(bankAccountFacade, categoryFacade, operationFacade)
        {
        }

        protected override ImportedData ReadDataFromFile(string filePath)
        {
            if (ShouldThrowOnRead)
                throw new Exception("Test exception");

            return new ImportedData
            {
                BankAccounts = new List<ImportedBankAccount>
                {
                    new()
                    {
                        Name = "Test Account",
                        Balance = 1000m
                    }
                },
                Categories = new List<ImportedCategory>
                {
                    new()
                    {
                        Type = CategoryType.Income,
                        Name = "Test Category"
                    }
                },
                Operations = new List<ImportedOperation>
                {
                    new()
                    {
                        Type = OperationType.Income,
                        BankAccountName = "Test Account",
                        Amount = 500m,
                        Date = DateTime.Now,
                        CategoryName = "Test Category",
                        Description = "Test Operation"
                    }
                }
            };
        }
    }
}