using HSEBank.FinancialAccounting.Interfaces.Facades;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;
using HSEBank.FinancialAccounting.Services;
using Moq;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Services;

public class ImportExportServiceTests
{
    private readonly Mock<IBankAccountFacade> _mockBankAccountFacade;
    private readonly Mock<ICategoryFacade> _mockCategoryFacade;
    private readonly Mock<IOperationFacade> _mockOperationFacade;
    private readonly ImportExportService _service;
    private readonly string _testFilePath = Path.GetTempFileName();
    private readonly List<BankAccount> _testBankAccounts;
    private readonly List<Category> _testCategories;
    private readonly List<Operation> _testOperations;

    public ImportExportServiceTests()
    {
        _mockBankAccountFacade = new Mock<IBankAccountFacade>();
        _mockCategoryFacade = new Mock<ICategoryFacade>();
        _mockOperationFacade = new Mock<IOperationFacade>();
        _service = new ImportExportService(
            _mockBankAccountFacade.Object,
            _mockCategoryFacade.Object,
            _mockOperationFacade.Object);

        _testBankAccounts = new List<BankAccount>
        {
            new(Guid.NewGuid(), "Test Bank Account", 1000m)
        };

        _testCategories = new List<Category>
        {
            new(Guid.NewGuid(), CategoryType.Income, "Test Income Category"),
            new(Guid.NewGuid(), CategoryType.Expense, "Test Expense Category")
        };

        var bankAccountId = _testBankAccounts[0].Id;
        var incomeCategoryId = _testCategories[0].Id;
        var expenseCategoryId = _testCategories[1].Id;

        _testOperations = new List<Operation>
        {
            new(Guid.NewGuid(), OperationType.Income, bankAccountId, 500m, DateTime.Now, incomeCategoryId, "Test Income"),
            new(Guid.NewGuid(), OperationType.Expense, bankAccountId, 200m, DateTime.Now, expenseCategoryId, "Test Expense")
        };
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    [Fact]
    public void Constructor_WithNullBankAccountFacade_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ImportExportService(
            null,
            _mockCategoryFacade.Object,
            _mockOperationFacade.Object));
    }

    [Fact]
    public void Constructor_WithNullCategoryFacade_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ImportExportService(
            _mockBankAccountFacade.Object,
            null,
            _mockOperationFacade.Object));
    }

    [Fact]
    public void Constructor_WithNullOperationFacade_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new ImportExportService(
            _mockBankAccountFacade.Object,
            _mockCategoryFacade.Object,
            null));
    }

    [Fact]
    public void ExportData_CallsAllFacadesAndExportsData()
    {
        // Arrange
        _mockBankAccountFacade.Setup(f => f.GetAllBankAccounts()).Returns(_testBankAccounts);
        _mockCategoryFacade.Setup(f => f.GetAllCategories()).Returns(_testCategories);
        _mockOperationFacade.Setup(f => f.GetAllOperations()).Returns(_testOperations);

        // Act
        try
        {
            _service.ExportData(_testFilePath, "json");

            // Assert
            _mockBankAccountFacade.Verify(f => f.GetAllBankAccounts(), Times.Once);
            _mockCategoryFacade.Verify(f => f.GetAllCategories(), Times.Once);
            _mockOperationFacade.Verify(f => f.GetAllOperations(), Times.Once);

            Assert.True(File.Exists(_testFilePath));
                
            var fileContent = File.ReadAllText(_testFilePath);
            Assert.NotEmpty(fileContent);
        }
        finally
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }
    }

    [Fact]
    public void ExportData_WithUnsupportedFormat_ThrowsArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => _service.ExportData(_testFilePath, "unsupported"));
    }

    [Fact]
    public void ImportData_WithNonExistentFile_ThrowsFileNotFoundException()
    {
        // Arrange
        string nonExistentFilePath = "non_existent_file.json";

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => _service.ImportData(nonExistentFilePath, "json"));
    }

    [Fact]
    public void ImportData_WithUnsupportedFormat_ThrowsArgumentException()
    {
        // Arrange
        File.WriteAllText(_testFilePath, "");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _service.ImportData(_testFilePath, "unsupported"));
    }

    [Fact]
    public void ExportBankAccounts_ExportsOnlyBankAccounts()
    {
        // Act
        try
        {
            _service.ExportBankAccounts(_testFilePath, "json", _testBankAccounts);

            // Assert
            Assert.True(File.Exists(_testFilePath));
                
            var fileContent = File.ReadAllText(_testFilePath);
            Assert.Contains("Test Bank Account", fileContent);
            Assert.Contains("1000", fileContent);
        }
        finally
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }
    }

    [Fact]
    public void ExportCategories_ExportsOnlyCategories()
    {
        // Act
        try
        {
            _service.ExportCategories(_testFilePath, "json", _testCategories);

            // Assert
            Assert.True(File.Exists(_testFilePath));
                
            var fileContent = File.ReadAllText(_testFilePath);
            Assert.Contains("Test Income Category", fileContent);
            Assert.Contains("Test Expense Category", fileContent);
        }
        finally
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }
    }

    [Fact]
    public void ExportOperations_ExportsOnlyOperations()
    {
        // Act
        try
        {
            _service.ExportOperations(_testFilePath, "json", _testOperations);

            // Assert
            Assert.True(File.Exists(_testFilePath));
                
            var fileContent = File.ReadAllText(_testFilePath);
            Assert.Contains("Test Income", fileContent);
            Assert.Contains("Test Expense", fileContent);
            Assert.Contains("500", fileContent);
            Assert.Contains("200", fileContent);
        }
        finally
        {
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }
    }
}