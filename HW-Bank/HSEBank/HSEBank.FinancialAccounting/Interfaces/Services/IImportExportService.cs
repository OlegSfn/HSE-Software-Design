using HSEBank.FinancialAccounting.Models;

namespace HSEBank.FinancialAccounting.Interfaces.Services;

public interface IImportExportService
{
    void ExportData(string filePath, string format);
    bool ImportData(string filePath, string format);
    void ExportBankAccounts(string filePath, string format, IEnumerable<BankAccount> bankAccounts);
    void ExportCategories(string filePath, string format, IEnumerable<Category> categories);
    void ExportOperations(string filePath, string format, IEnumerable<Operation> operations);
}