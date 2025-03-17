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
            try
            {
                var serviceProvider = DependencyInjection.ConfigureServices();

                var bankAccountFacade = serviceProvider.GetRequiredService<IBankAccountFacade>();
                var categoryFacade = serviceProvider.GetRequiredService<ICategoryFacade>();
                var operationFacade = serviceProvider.GetRequiredService<IOperationFacade>();
                var analyticsFacade = serviceProvider.GetRequiredService<IAnalyticsFacade>();

                var consoleUI = new ConsoleUI(bankAccountFacade, categoryFacade, operationFacade, analyticsFacade);
                consoleUI.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
