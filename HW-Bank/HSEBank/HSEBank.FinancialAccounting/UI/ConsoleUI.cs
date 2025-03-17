using HSEBank.FinancialAccounting.Commands;
using HSEBank.FinancialAccounting.Decorators;
using HSEBank.FinancialAccounting.Interfaces;
using HSEBank.FinancialAccounting.Interfaces.Facades;
using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.UI;

public class ConsoleUI
{
    private readonly IBankAccountFacade _bankAccountFacade;
    private readonly ICategoryFacade _categoryFacade;
    private readonly IOperationFacade _operationFacade;
    private readonly IAnalyticsFacade _analyticsFacade;
    private readonly Dictionary<string, Action> _menuActions;
    private bool _isRunning = true;

    public ConsoleUI(
        IBankAccountFacade bankAccountFacade,
        ICategoryFacade categoryFacade,
        IOperationFacade operationFacade,
        IAnalyticsFacade analyticsFacade)
    {
        _bankAccountFacade = bankAccountFacade ?? throw new ArgumentNullException(nameof(bankAccountFacade));
        _categoryFacade = categoryFacade ?? throw new ArgumentNullException(nameof(categoryFacade));
        _operationFacade = operationFacade ?? throw new ArgumentNullException(nameof(operationFacade));
        _analyticsFacade = analyticsFacade ?? throw new ArgumentNullException(nameof(analyticsFacade));

        _menuActions = new Dictionary<string, Action>
        {
            { "1", ShowBankAccounts },
            { "2", CreateBankAccount },
            { "3", UpdateBankAccount },
            { "4", DeleteBankAccount },
            { "5", ShowCategories },
            { "6", CreateCategory },
            { "7", UpdateCategory },
            { "8", DeleteCategory },
            { "9", ShowOperations },
            { "10", CreateOperation },
            { "11", DeleteOperation },
            { "12", ShowAnalytics },
            { "13", Exit }
        };
    }

    public void Run()
    {
        Console.WriteLine("Welcome to HSE Bank!");
        Console.WriteLine();

        while (_isRunning)
        {
            ShowMenu();
            var choice = Console.ReadLine();
            Console.WriteLine();

            if (_menuActions.TryGetValue(choice, out var action))
            {
                action();
            }
            else
            {
                Console.WriteLine("Invalid choice. Please try again.");
            }
        }
    }

    private void ShowMenu()
    {
        Console.WriteLine("=== MAIN MENU ===");

        Console.WriteLine("\n=== Bank Accounts ===");
        Console.WriteLine("1. Show Bank Accounts");
        Console.WriteLine("2. Create Bank Account");
        Console.WriteLine("3. Update Bank Account");
        Console.WriteLine("4. Delete Bank Account");

        Console.WriteLine("\n=== Categories ===");
        Console.WriteLine("5. Show Categories");
        Console.WriteLine("6. Create Category");
        Console.WriteLine("7. Update Category");
        Console.WriteLine("8. Delete Category");

        Console.WriteLine("\n=== Operations ===");
        Console.WriteLine("9. Show Operations");
        Console.WriteLine("10. Create Operation");
        Console.WriteLine("11. Delete Operation");

        Console.WriteLine("");
        Console.WriteLine("12. Show Analytics");
        Console.WriteLine("13. Exit");
        Console.Write("Enter your choice: ");
    }

    private void ShowBankAccounts()
    {
        var bankAccounts = _bankAccountFacade.GetAllBankAccounts().ToList();
        if (bankAccounts.Count == 0)
        {
            Console.WriteLine("No bank accounts found.");
            return;
        }

        Console.WriteLine("=== BANK ACCOUNTS ===");
        foreach (var bankAccount in bankAccounts)
        {
            Console.WriteLine($"ID: {bankAccount.Id}");
            Console.WriteLine($"Name: {bankAccount.Name}");
            Console.WriteLine($"Balance: {bankAccount.Balance:C}");
            Console.WriteLine();
        }
    }

    private void CreateBankAccount()
    {
        Console.WriteLine("=== CREATE BANK ACCOUNT ===");
        Console.Write("Enter account name: ");
        var name = Console.ReadLine();

        Console.Write("Enter initial balance: ");
        if (!decimal.TryParse(Console.ReadLine(), out var initialBalance))
        {
            Console.WriteLine("Invalid balance. Please enter a valid number.");
            return;
        }

        try
        {
            var command = new CreateBankAccountCommand(_bankAccountFacade, name, initialBalance);
            var timedCommand = new CommandTimingDecorator(command, LogExecutionTime);
            timedCommand.Execute();

            Console.WriteLine("Bank account created successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating bank account: {ex.Message}");
        }
    }

    private void ShowCategories()
    {
        var categories = _categoryFacade.GetAllCategories().ToList();
        if (categories.Count == 0)
        {
            Console.WriteLine("No categories found.");
            return;
        }

        Console.WriteLine("=== CATEGORIES ===");
        foreach (var category in categories)
        {
            Console.WriteLine($"ID: {category.Id}");
            Console.WriteLine($"Name: {category.Name}");
            Console.WriteLine($"Type: {category.Type}");
            Console.WriteLine();
        }
    }

    private void CreateCategory()
    {
        Console.WriteLine("=== CREATE CATEGORY ===");
        Console.Write("Enter category name: ");
        var name = Console.ReadLine();

        Console.WriteLine("Select category type:");
        Console.WriteLine("1. Income");
        Console.WriteLine("2. Expense");
        Console.Write("Enter your choice: ");
        var typeChoice = Console.ReadLine();

        CategoryType type;
        switch (typeChoice)
        {
            case "1":
                type = CategoryType.Income;
                break;
            case "2":
                type = CategoryType.Expense;
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                return;
        }

        try
        {
            var command = new CreateCategoryCommand(_categoryFacade, type, name);
            var timedCommand = new CommandTimingDecorator(command, LogExecutionTime);
            timedCommand.Execute();

            Console.WriteLine("Category created successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating category: {ex.Message}");
        }
    }

    private void ShowOperations()
    {
        var operations = _operationFacade.GetAllOperations().ToList();
        if (operations.Count == 0)
        {
            Console.WriteLine("No operations found.");
            return;
        }

        Console.WriteLine("=== OPERATIONS ===");
        foreach (var operation in operations)
        {
            var bankAccount = _bankAccountFacade.GetBankAccountById(operation.BankAccountId);
            var category = _categoryFacade.GetCategoryById(operation.CategoryId);

            Console.WriteLine($"ID: {operation.Id}");
            Console.WriteLine($"Type: {operation.Type}");
            Console.WriteLine($"Bank Account: {bankAccount?.Name ?? "Unknown"}");
            Console.WriteLine($"Category: {category?.Name ?? "Unknown"}");
            Console.WriteLine($"Amount: {operation.Amount:C}");
            Console.WriteLine($"Date: {operation.Date}");
            if (!string.IsNullOrEmpty(operation.Description))
            {
                Console.WriteLine($"Description: {operation.Description}");
            }
            Console.WriteLine();
        }
    }

    private void CreateOperation()
    {
        Console.WriteLine("=== CREATE OPERATION ===");

        var bankAccounts = _bankAccountFacade.GetAllBankAccounts().ToList();
        if (bankAccounts.Count == 0)
        {
            Console.WriteLine("No bank accounts found. Please create a bank account first.");
            return;
        }

        Console.WriteLine("Select bank account:");
        for (int i = 0; i < bankAccounts.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {bankAccounts[i].Name} (Balance: {bankAccounts[i].Balance:C})");
        }
        Console.Write("Enter your choice: ");
        if (!int.TryParse(Console.ReadLine(), out var bankAccountChoice) || bankAccountChoice < 1 || bankAccountChoice > bankAccounts.Count)
        {
            Console.WriteLine("Invalid choice. Please try again.");
            return;
        }
        var bankAccount = bankAccounts[bankAccountChoice - 1];

        Console.WriteLine("Select operation type:");
        Console.WriteLine("1. Income");
        Console.WriteLine("2. Expense");
        Console.Write("Enter your choice: ");
        var typeChoice = Console.ReadLine();

        OperationType type;
        switch (typeChoice)
        {
            case "1":
                type = OperationType.Income;
                break;
            case "2":
                type = OperationType.Expense;
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                return;
        }

        var categories = _categoryFacade.GetCategoriesByType((CategoryType)type).ToList();
        if (categories.Count == 0)
        {
            Console.WriteLine($"No {type} categories found. Please create a category first.");
            return;
        }

        Console.WriteLine($"Select {type} category:");
        for (int i = 0; i < categories.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {categories[i].Name}");
        }
        Console.Write("Enter your choice: ");
        if (!int.TryParse(Console.ReadLine(), out var categoryChoice) || categoryChoice < 1 || categoryChoice > categories.Count)
        {
            Console.WriteLine("Invalid choice. Please try again.");
            return;
        }
        var category = categories[categoryChoice - 1];

        Console.Write("Enter amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
        {
            Console.WriteLine("Invalid amount. Please enter a positive number.");
            return;
        }

        Console.Write("Enter date (yyyy-MM-dd) or leave empty for today: ");
        var dateInput = Console.ReadLine();
        DateTime date;
        if (string.IsNullOrWhiteSpace(dateInput))
        {
            date = DateTime.Today;
        }
        else if (!DateTime.TryParse(dateInput, out date))
        {
            Console.WriteLine("Invalid date format. Please try again.");
            return;
        }

        Console.Write("Enter description (optional): ");
        var description = Console.ReadLine();

        try
        {
            var command = new CreateOperationCommand(_operationFacade, type, bankAccount.Id, amount, date, category.Id, description);
            var timedCommand = new CommandTimingDecorator(command, LogExecutionTime);
            timedCommand.Execute();

            Console.WriteLine("Operation created successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating operation: {ex.Message}");
        }
    }

    private void ShowAnalytics()
    {
        Console.WriteLine("=== ANALYTICS ===");

        var bankAccounts = _bankAccountFacade.GetAllBankAccounts().ToList();
        if (bankAccounts.Count == 0)
        {
            Console.WriteLine("No bank accounts found. Please create a bank account first.");
            return;
        }

        Console.WriteLine("Select bank account:");
        for (int i = 0; i < bankAccounts.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {bankAccounts[i].Name} (Balance: {bankAccounts[i].Balance:C})");
        }
        Console.Write("Enter your choice: ");
        if (!int.TryParse(Console.ReadLine(), out var bankAccountChoice) || bankAccountChoice < 1 || bankAccountChoice > bankAccounts.Count)
        {
            Console.WriteLine("Invalid choice. Please try again.");
            return;
        }
        var bankAccount = bankAccounts[bankAccountChoice - 1];

        Console.WriteLine("Select date range:");
        Console.WriteLine("1. This month");
        Console.WriteLine("2. Last month");
        Console.WriteLine("3. This year");
        Console.WriteLine("4. Custom range");
        Console.Write("Enter your choice: ");
        var rangeChoice = Console.ReadLine();

        DateTime startDate, endDate;
        switch (rangeChoice)
        {
            case "1":
                startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);
                break;
            case "2":
                startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-1);
                endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);
                break;
            case "3":
                startDate = new DateTime(DateTime.Today.Year, 1, 1);
                endDate = new DateTime(DateTime.Today.Year, 12, 31);
                break;
            case "4":
                Console.Write("Enter start date (yyyy-MM-dd): ");
                if (!DateTime.TryParse(Console.ReadLine(), out startDate))
                {
                    Console.WriteLine("Invalid date format. Please try again.");
                    return;
                }
                Console.Write("Enter end date (yyyy-MM-dd): ");
                if (!DateTime.TryParse(Console.ReadLine(), out endDate))
                {
                    Console.WriteLine("Invalid date format. Please try again.");
                    return;
                }
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                return;
        }

        Console.WriteLine();
        Console.WriteLine($"Analytics for {bankAccount.Name} from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");
        Console.WriteLine();

        var totalIncome = _analyticsFacade.CalculateTotalIncome(bankAccount.Id, startDate, endDate);
        var totalExpenses = _analyticsFacade.CalculateTotalExpenses(bankAccount.Id, startDate, endDate);
        var difference = _analyticsFacade.CalculateIncomeExpenseDifference(bankAccount.Id, startDate, endDate);

        Console.WriteLine($"Total Income: {totalIncome:C}");
        Console.WriteLine($"Total Expenses: {totalExpenses:C}");
        Console.WriteLine($"Difference: {difference:C}");
        Console.WriteLine();

        var categoryGroups = _analyticsFacade.GroupOperationsByCategory(bankAccount.Id, startDate, endDate);
        if (categoryGroups.Count > 0)
        {
            Console.WriteLine("Operations by Category:");
            foreach (var group in categoryGroups)
            {
                Console.WriteLine($"{group.Key.Name} ({group.Key.Type}): {group.Value:C}");
            }
        }
    }

    private void UpdateBankAccount()
    {
        Console.WriteLine("=== UPDATE BANK ACCOUNT ===");
        
        var bankAccounts = _bankAccountFacade.GetAllBankAccounts().ToList();
        if (bankAccounts.Count == 0)
        {
            Console.WriteLine("No bank accounts found.");
            return;
        }

        Console.WriteLine("Select bank account to update:");
        for (int i = 0; i < bankAccounts.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {bankAccounts[i].Name} (Balance: {bankAccounts[i].Balance:C})");
        }
        Console.Write("Enter your choice: ");
        if (!int.TryParse(Console.ReadLine(), out var bankAccountChoice) || bankAccountChoice < 1 || bankAccountChoice > bankAccounts.Count)
        {
            Console.WriteLine("Invalid choice. Please try again.");
            return;
        }
        var bankAccount = bankAccounts[bankAccountChoice - 1];

        Console.WriteLine("What do you want to update?");
        Console.WriteLine("1. Name");
        Console.WriteLine("2. Balance");
        Console.Write("Enter your choice: ");
        var updateChoice = Console.ReadLine();

        try
        {
            switch (updateChoice)
            {
                case "1":
                    Console.Write("Enter new name: ");
                    var newName = Console.ReadLine();
                    _bankAccountFacade.UpdateBankAccountName(bankAccount.Id, newName);
                    Console.WriteLine("Bank account name updated successfully.");
                    break;
                case "2":
                    Console.Write("Enter new balance: ");
                    if (!decimal.TryParse(Console.ReadLine(), out var newBalance))
                    {
                        Console.WriteLine("Invalid balance. Please enter a valid number.");
                        return;
                    }
                    _bankAccountFacade.UpdateBankAccountBalance(bankAccount.Id, newBalance);
                    Console.WriteLine("Bank account balance updated successfully.");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating bank account: {ex.Message}");
        }
    }

    private void DeleteBankAccount()
    {
        Console.WriteLine("=== DELETE BANK ACCOUNT ===");
        
        var bankAccounts = _bankAccountFacade.GetAllBankAccounts().ToList();
        if (bankAccounts.Count == 0)
        {
            Console.WriteLine("No bank accounts found.");
            return;
        }

        Console.WriteLine("Select bank account to delete:");
        for (int i = 0; i < bankAccounts.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {bankAccounts[i].Name} (Balance: {bankAccounts[i].Balance:C})");
        }
        Console.Write("Enter your choice: ");
        if (!int.TryParse(Console.ReadLine(), out var bankAccountChoice) || bankAccountChoice < 1 || bankAccountChoice > bankAccounts.Count)
        {
            Console.WriteLine("Invalid choice. Please try again.");
            return;
        }
        var bankAccount = bankAccounts[bankAccountChoice - 1];

        Console.Write("Are you sure you want to delete this bank account? (y/n): ");
        var confirmation = Console.ReadLine()?.ToLower();
        if (confirmation != "y" && confirmation != "yes")
        {
            Console.WriteLine("Operation cancelled.");
            return;
        }

        try
        {
            var result = _bankAccountFacade.DeleteBankAccount(bankAccount.Id);
            if (result)
            {
                Console.WriteLine("Bank account deleted successfully.");
            }
            else
            {
                Console.WriteLine("Failed to delete bank account.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting bank account: {ex.Message}");
        }
    }

    private void UpdateCategory()
    {
        Console.WriteLine("=== UPDATE CATEGORY ===");
        
        var categories = _categoryFacade.GetAllCategories().ToList();
        if (categories.Count == 0)
        {
            Console.WriteLine("No categories found.");
            return;
        }

        Console.WriteLine("Select category to update:");
        for (int i = 0; i < categories.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {categories[i].Name} ({categories[i].Type})");
        }
        Console.Write("Enter your choice: ");
        if (!int.TryParse(Console.ReadLine(), out var categoryChoice) || categoryChoice < 1 || categoryChoice > categories.Count)
        {
            Console.WriteLine("Invalid choice. Please try again.");
            return;
        }
        var category = categories[categoryChoice - 1];

        Console.WriteLine("What do you want to update?");
        Console.WriteLine("1. Name");
        Console.WriteLine("2. Type");
        Console.Write("Enter your choice: ");
        var updateChoice = Console.ReadLine();

        try
        {
            switch (updateChoice)
            {
                case "1":
                    Console.Write("Enter new name: ");
                    var newName = Console.ReadLine();
                    _categoryFacade.UpdateCategoryName(category.Id, newName);
                    Console.WriteLine("Category name updated successfully.");
                    break;
                case "2":
                    Console.WriteLine("Select new category type:");
                    Console.WriteLine("1. Income");
                    Console.WriteLine("2. Expense");
                    Console.Write("Enter your choice: ");
                    var typeChoice = Console.ReadLine();

                    CategoryType newType;
                    switch (typeChoice)
                    {
                        case "1":
                            newType = CategoryType.Income;
                            break;
                        case "2":
                            newType = CategoryType.Expense;
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            return;
                    }

                    _categoryFacade.UpdateCategoryType(category.Id, newType);
                    Console.WriteLine("Category type updated successfully.");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating category: {ex.Message}");
        }
    }

    private void DeleteCategory()
    {
        Console.WriteLine("=== DELETE CATEGORY ===");
        
        var categories = _categoryFacade.GetAllCategories().ToList();
        if (categories.Count == 0)
        {
            Console.WriteLine("No categories found.");
            return;
        }

        Console.WriteLine("Select category to delete:");
        for (int i = 0; i < categories.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {categories[i].Name} ({categories[i].Type})");
        }
        Console.Write("Enter your choice: ");
        if (!int.TryParse(Console.ReadLine(), out var categoryChoice) || categoryChoice < 1 || categoryChoice > categories.Count)
        {
            Console.WriteLine("Invalid choice. Please try again.");
            return;
        }
        var category = categories[categoryChoice - 1];

        Console.Write("Are you sure you want to delete this category? (y/n): ");
        var confirmation = Console.ReadLine()?.ToLower();
        if (confirmation != "y" && confirmation != "yes")
        {
            Console.WriteLine("Operation cancelled.");
            return;
        }

        try
        {
            var result = _categoryFacade.DeleteCategory(category.Id);
            if (result)
            {
                Console.WriteLine("Category deleted successfully.");
            }
            else
            {
                Console.WriteLine("Failed to delete category.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting category: {ex.Message}");
        }
    }

    private void DeleteOperation()
    {
        Console.WriteLine("=== DELETE OPERATION ===");
        
        var operations = _operationFacade.GetAllOperations().ToList();
        if (operations.Count == 0)
        {
            Console.WriteLine("No operations found.");
            return;
        }

        Console.WriteLine("Select operation to delete:");
        for (int i = 0; i < operations.Count; i++)
        {
            var bankAccount = _bankAccountFacade.GetBankAccountById(operations[i].BankAccountId);
            var category = _categoryFacade.GetCategoryById(operations[i].CategoryId);
            
            Console.WriteLine($"{i + 1}. {operations[i].Type} - {category?.Name ?? "Unknown"} - {operations[i].Amount:C} ({bankAccount?.Name ?? "Unknown"}, {operations[i].Date:yyyy-MM-dd})");
        }
        Console.Write("Enter your choice: ");
        if (!int.TryParse(Console.ReadLine(), out var operationChoice) || operationChoice < 1 || operationChoice > operations.Count)
        {
            Console.WriteLine("Invalid choice. Please try again.");
            return;
        }
        var operation = operations[operationChoice - 1];

        Console.Write("Are you sure you want to delete this operation? (y/n): ");
        var confirmation = Console.ReadLine()?.ToLower();
        if (confirmation != "y" && confirmation != "yes")
        {
            Console.WriteLine("Operation cancelled.");
            return;
        }

        try
        {
            var result = _operationFacade.DeleteOperation(operation.Id);
            if (result)
            {
                Console.WriteLine("Operation deleted successfully.");
            }
            else
            {
                Console.WriteLine("Failed to delete operation.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting operation: {ex.Message}");
        }
    }

    private void LogExecutionTime(string commandName, TimeSpan executionTime)
    {
        Console.WriteLine($"Command {commandName} executed in {executionTime.TotalMilliseconds:F2} ms");
    }

    private void Exit()
    {
        _isRunning = false;
    }
}
