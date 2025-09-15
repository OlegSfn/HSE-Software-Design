using HSEBank.FinancialAccounting.DI;
using HSEBank.FinancialAccounting.Interfaces.Facades;
using HSEBank.FinancialAccounting.UI;
using Microsoft.Extensions.DependencyInjection;

namespace HSEBank.FinancialAccounting
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = DependencyInjection.ConfigureServices();

            var bankAccountFacade = serviceProvider.GetRequiredService<IBankAccountFacade>();
            var categoryFacade = serviceProvider.GetRequiredService<ICategoryFacade>();
            var operationFacade = serviceProvider.GetRequiredService<IOperationFacade>();
            var analyticsFacade = serviceProvider.GetRequiredService<IAnalyticsFacade>();

            var consoleUI = new ConsoleUI(bankAccountFacade, categoryFacade, operationFacade, analyticsFacade);
            consoleUI.Run();
        }
    }
}
