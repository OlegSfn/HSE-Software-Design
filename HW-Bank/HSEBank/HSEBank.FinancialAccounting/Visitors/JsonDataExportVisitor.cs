using System.Text.Json;
using HSEBank.FinancialAccounting.Models;

namespace HSEBank.FinancialAccounting.Visitors;

public class JsonDataExportVisitor : IDataExportVisitor
{
    private readonly List<ExportedBankAccount> _bankAccounts = new();
    private readonly List<ExportedCategory> _categories = new();
    private readonly List<ExportedOperation> _operations = new();

    public void VisitBankAccounts(IEnumerable<BankAccount> bankAccounts)
    {
        foreach (var bankAccount in bankAccounts)
        {
            _bankAccounts.Add(new ExportedBankAccount
            {
                Id = bankAccount.Id,
                Name = bankAccount.Name,
                Balance = bankAccount.Balance
            });
        }
    }

    public void VisitCategories(IEnumerable<Category> categories)
    {
        foreach (var category in categories)
        {
            _categories.Add(new ExportedCategory
            {
                Id = category.Id,
                Type = category.Type,
                Name = category.Name
            });
        }
    }

    public void VisitOperations(IEnumerable<Operation> operations)
    {
        foreach (var operation in operations)
        {
            _operations.Add(new ExportedOperation
            {
                Id = operation.Id,
                Type = operation.Type,
                BankAccountId = operation.BankAccountId,
                Amount = operation.Amount,
                Date = operation.Date,
                CategoryId = operation.CategoryId,
                Description = operation.Description
            });
        }
    }
    
    public void ExportToFile(string filePath)
    {
        var exportedData = new ExportedData
        {
            BankAccounts = _bankAccounts,
            Categories = _categories,
            Operations = _operations
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(exportedData, options);
        File.WriteAllText(filePath, json);
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
        public Models.Enums.CategoryType Type { get; set; }
        public string Name { get; set; }
    }

    private class ExportedOperation
    {
        public Guid Id { get; set; }
        public Models.Enums.OperationType Type { get; set; }
        public Guid BankAccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Guid CategoryId { get; set; }
        public string Description { get; set; }
    }
}