using HSEBank.FinancialAccounting.Models;

namespace HSEBank.FinancialAccounting.Visitors;

public interface IDataExportVisitor
{
    void VisitBankAccounts(IEnumerable<BankAccount> bankAccounts);
    void VisitCategories(IEnumerable<Category> categories);
    void VisitOperations(IEnumerable<Operation> operations);
    void ExportToFile(string filePath);
}