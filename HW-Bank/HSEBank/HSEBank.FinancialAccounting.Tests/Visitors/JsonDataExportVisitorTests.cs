using System.Text.Json;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;
using HSEBank.FinancialAccounting.Visitors;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Visitors;

public class JsonDataExportVisitorTests : IDisposable
{
    private readonly JsonDataExportVisitor _visitor;
    private readonly List<BankAccount> _bankAccounts;
    private readonly List<Category> _categories;
    private readonly List<Operation> _operations;
    private readonly string _testFilePath;

    public JsonDataExportVisitorTests()
    {
        _visitor = new JsonDataExportVisitor();
            
        _bankAccounts = new List<BankAccount>
        {
            new(Guid.NewGuid(), "Account 1", 1000m),
            new(Guid.NewGuid(), "Account 2", 2000m)
        };
            
        var incomeCategory = new Category(Guid.NewGuid(), CategoryType.Income, "Salary");
        var expenseCategory = new Category(Guid.NewGuid(), CategoryType.Expense, "Cafe");
        _categories = new List<Category> { incomeCategory, expenseCategory };
            
        _operations = new List<Operation>
        {
            new(Guid.NewGuid(), OperationType.Income, _bankAccounts[0].Id, 500m, DateTime.Now, incomeCategory.Id, "Salary"),
            new(Guid.NewGuid(), OperationType.Expense, _bankAccounts[0].Id, 200m, DateTime.Now, expenseCategory.Id, "Lunch")
        };
            
        _testFilePath = Path.Combine(Path.GetTempPath(), $"test_export_{Guid.NewGuid()}.json");
    }

    public void Dispose()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    [Fact]
    public void VisitBankAccounts_AddsBankAccountsToVisitor()
    {
        // Act
        _visitor.VisitBankAccounts(_bankAccounts);
        _visitor.ExportToFile(_testFilePath);
            
        // Assert
        Assert.True(File.Exists(_testFilePath));
        var json = File.ReadAllText(_testFilePath);
        var exportedData = JsonSerializer.Deserialize<ExportedData>(json);
            
        Assert.NotNull(exportedData);
        Assert.Equal(2, exportedData.BankAccounts.Count);
            
        for (int i = 0; i < _bankAccounts.Count; i++)
        {
            Assert.Equal(_bankAccounts[i].Id, exportedData.BankAccounts[i].Id);
            Assert.Equal(_bankAccounts[i].Name, exportedData.BankAccounts[i].Name);
            Assert.Equal(_bankAccounts[i].Balance, exportedData.BankAccounts[i].Balance);
        }
    }

    [Fact]
    public void VisitCategories_AddsCategoriestoVisitor()
    {
        // Act
        _visitor.VisitCategories(_categories);
        _visitor.ExportToFile(_testFilePath);
            
        // Assert
        Assert.True(File.Exists(_testFilePath));
        var json = File.ReadAllText(_testFilePath);
        var exportedData = JsonSerializer.Deserialize<ExportedData>(json);
            
        Assert.NotNull(exportedData);
        Assert.Equal(2, exportedData.Categories.Count);
            
        for (int i = 0; i < _categories.Count; i++)
        {
            Assert.Equal(_categories[i].Id, exportedData.Categories[i].Id);
            Assert.Equal(_categories[i].Name, exportedData.Categories[i].Name);
            Assert.Equal(_categories[i].Type, exportedData.Categories[i].Type);
        }
    }

    [Fact]
    public void VisitOperations_AddsOperationsToVisitor()
    {
        // Act
        _visitor.VisitOperations(_operations);
        _visitor.ExportToFile(_testFilePath);
            
        // Assert
        Assert.True(File.Exists(_testFilePath));
        var json = File.ReadAllText(_testFilePath);
        var exportedData = JsonSerializer.Deserialize<ExportedData>(json);
            
        Assert.NotNull(exportedData);
        Assert.Equal(2, exportedData.Operations.Count);
            
        for (int i = 0; i < _operations.Count; i++)
        {
            Assert.Equal(_operations[i].Id, exportedData.Operations[i].Id);
            Assert.Equal(_operations[i].Type, exportedData.Operations[i].Type);
            Assert.Equal(_operations[i].BankAccountId, exportedData.Operations[i].BankAccountId);
            Assert.Equal(_operations[i].Amount, exportedData.Operations[i].Amount);
            Assert.Equal(_operations[i].CategoryId, exportedData.Operations[i].CategoryId);
            Assert.Equal(_operations[i].Description, exportedData.Operations[i].Description);
        }
    }

    [Fact]
    public void ExportToFile_WithAllData_CreatesCorrectJsonFile()
    {
        // Act
        _visitor.VisitBankAccounts(_bankAccounts);
        _visitor.VisitCategories(_categories);
        _visitor.VisitOperations(_operations);
        _visitor.ExportToFile(_testFilePath);
            
        // Assert
        Assert.True(File.Exists(_testFilePath));
        var json = File.ReadAllText(_testFilePath);
        var exportedData = JsonSerializer.Deserialize<ExportedData>(json);
            
        Assert.NotNull(exportedData);
        Assert.Equal(2, exportedData.BankAccounts.Count);
        Assert.Equal(2, exportedData.Categories.Count);
        Assert.Equal(2, exportedData.Operations.Count);
    }

    [Fact]
    public void ExportToFile_WithNoData_CreatesEmptyJsonFile()
    {
        // Act
        _visitor.ExportToFile(_testFilePath);
            
        // Assert
        Assert.True(File.Exists(_testFilePath));
        var json = File.ReadAllText(_testFilePath);
        var exportedData = JsonSerializer.Deserialize<ExportedData>(json);
            
        Assert.NotNull(exportedData);
        Assert.Empty(exportedData.BankAccounts);
        Assert.Empty(exportedData.Categories);
        Assert.Empty(exportedData.Operations);
    }

    private class ExportedData
    {
        public List<ExportedBankAccount> BankAccounts { get; set; }
        public List<ExportedCategory> Categories { get; set; }
        public List<ExportedOperation> Operations { get; set; }
    }

    private class ExportedBankAccount
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
    }

    private class ExportedCategory
    {
        public Guid Id { get; set; }
        public CategoryType Type { get; set; }
        public string Name { get; set; }
    }

    private class ExportedOperation
    {
        public Guid Id { get; set; }
        public OperationType Type { get; set; }
        public Guid BankAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Guid CategoryId { get; set; }
        public string Description { get; set; }
    }
}