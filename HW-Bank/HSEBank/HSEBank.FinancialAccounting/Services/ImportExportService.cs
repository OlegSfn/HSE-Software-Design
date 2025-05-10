using HSEBank.FinancialAccounting.Interfaces;
using HSEBank.FinancialAccounting.Interfaces.Facades;
using HSEBank.FinancialAccounting.Interfaces.Services;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.TemplateMethod;
using HSEBank.FinancialAccounting.Visitors;

namespace HSEBank.FinancialAccounting.Services;

public class ImportExportService : IImportExportService
{
    private readonly IBankAccountFacade _bankAccountFacade;
    private readonly ICategoryFacade _categoryFacade;
    private readonly IOperationFacade _operationFacade;

    public ImportExportService(IBankAccountFacade bankAccountFacade, ICategoryFacade categoryFacade, IOperationFacade operationFacade)
    {
        _bankAccountFacade = bankAccountFacade ?? throw new ArgumentNullException(nameof(bankAccountFacade));
        _categoryFacade = categoryFacade ?? throw new ArgumentNullException(nameof(categoryFacade));
        _operationFacade = operationFacade ?? throw new ArgumentNullException(nameof(operationFacade));
    }

    public void ExportData(string filePath, string format)
    {
        var bankAccounts = _bankAccountFacade.GetAllBankAccounts();
        var categories = _categoryFacade.GetAllCategories();
        var operations = _operationFacade.GetAllOperations();

        ExportData(filePath, format, bankAccounts, categories, operations);
    }

    public bool ImportData(string filePath, string format)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }

        DataImporter? importer;

        switch (format.ToLower())
        {
            case "json":
                importer = new JsonDataImporter(_bankAccountFacade, _categoryFacade, _operationFacade);
                break;
            default:
                throw new ArgumentException($"Unsupported format: {format}");
        }

        return importer.ImportData(filePath);
    }

    public void ExportBankAccounts(string filePath, string format, IEnumerable<BankAccount> bankAccounts)
        => ExportData(filePath, format, bankAccounts, new List<Category>(), new List<Operation>());
    
    public void ExportCategories(string filePath, string format, IEnumerable<Category> categories)
    => ExportData(filePath, format, new List<BankAccount>(), categories, new List<Operation>());
    
    public void ExportOperations(string filePath, string format, IEnumerable<Operation> operations)
        => ExportData(filePath, format, new List<BankAccount>(), new List<Category>(), operations);

    private void ExportData(string filePath, string format, IEnumerable<BankAccount> bankAccounts, IEnumerable<Category> categories, IEnumerable<Operation> operations)
    {
        IDataExportVisitor? visitor;

        switch (format.ToLower())
        {
            case "json":
                visitor = new JsonDataExportVisitor();
                break;
            default:
                throw new ArgumentException($"Unsupported format: {format}");
        }

        visitor.VisitBankAccounts(bankAccounts);
        visitor.VisitCategories(categories);
        visitor.VisitOperations(operations);
        visitor.ExportToFile(filePath);
    }
}