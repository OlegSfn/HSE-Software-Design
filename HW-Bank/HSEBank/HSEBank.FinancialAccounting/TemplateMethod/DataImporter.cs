using HSEBank.FinancialAccounting.Interfaces.Facades;

namespace HSEBank.FinancialAccounting.TemplateMethod;

public abstract class DataImporter
{
    protected readonly IBankAccountFacade BankAccountFacade;
    protected readonly ICategoryFacade CategoryFacade;
    protected readonly IOperationFacade OperationFacade;

    protected DataImporter(IBankAccountFacade bankAccountFacade, ICategoryFacade categoryFacade, IOperationFacade operationFacade)
    {
        BankAccountFacade = bankAccountFacade ?? throw new ArgumentNullException(nameof(bankAccountFacade));
        CategoryFacade = categoryFacade ?? throw new ArgumentNullException(nameof(categoryFacade));
        OperationFacade = operationFacade ?? throw new ArgumentNullException(nameof(operationFacade));
    }

    public bool ImportData(string filePath)
    {
        try
        {
            var data = ReadDataFromFile(filePath);

            foreach (var bankAccount in data.BankAccounts)
            {
                BankAccountFacade.CreateBankAccount(bankAccount.Name, bankAccount.Balance);
            }

            foreach (var category in data.Categories)
            {
                CategoryFacade.CreateCategory(category.Type, category.Name);
            }

            foreach (var operation in data.Operations)
            {
                var bankAccount = BankAccountFacade.GetBankAccountByName(operation.BankAccountName);
                if (bankAccount == null)
                {
                    continue;
                }

                var category = CategoryFacade.GetCategoryByName(operation.CategoryName);
                if (category == null)
                {
                    continue;
                }

                OperationFacade.CreateOperation(
                    operation.Type,
                    bankAccount.Id,
                    operation.Amount,
                    operation.Date,
                    category.Id,
                    operation.Description);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    protected abstract ImportedData ReadDataFromFile(string filePath);

    protected class ImportedData
    {
        public List<ImportedBankAccount> BankAccounts { get; set; } = new();
        public List<ImportedCategory> Categories { get; set; } = new();
        public List<ImportedOperation> Operations { get; set; } = new();
    }

    protected class ImportedBankAccount
    {
        public string Name { get; set; }
        public decimal Balance { get; set; }
    }

    protected class ImportedCategory
    {
        public Models.Enums.CategoryType Type { get; set; }
        public string Name { get; set; }
    }

    protected class ImportedOperation
    {
        public Models.Enums.OperationType Type { get; set; }
        public string BankAccountName { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
    }
}