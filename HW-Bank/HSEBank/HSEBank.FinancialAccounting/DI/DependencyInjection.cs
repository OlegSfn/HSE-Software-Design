using Microsoft.Extensions.DependencyInjection;
using HSEBank.FinancialAccounting.Factories;
using HSEBank.FinancialAccounting.Facades;
using HSEBank.FinancialAccounting.Interfaces.Facades;
using HSEBank.FinancialAccounting.Interfaces.Factories;
using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Interfaces.Services;
using HSEBank.FinancialAccounting.Repositories;
using HSEBank.FinancialAccounting.Services;
using HSEBank.FinancialAccounting.Proxies;

namespace HSEBank.FinancialAccounting.DI;

public static class DependencyInjection
{
    public static IServiceProvider ConfigureServices(bool useCache = true)
    {
        var services = new ServiceCollection();

        services.AddSingleton<IBankAccountFactory, BankAccountFactory>();
        services.AddSingleton<ICategoryFactory, CategoryFactory>();
        services.AddSingleton<IOperationFactory, OperationFactory>();

        if (useCache)
        {
            services.AddSingleton<InMemoryBankAccountRepository>();
            services.AddSingleton<InMemoryCategoryRepository>();
            services.AddSingleton<InMemoryOperationRepository>();

            services.AddSingleton<IBankAccountRepository>(provider =>
                new CachedBankAccountRepository(provider.GetRequiredService<InMemoryBankAccountRepository>()));
            services.AddSingleton<ICategoryRepository>(provider =>
                new CachedCategoryRepository(provider.GetRequiredService<InMemoryCategoryRepository>()));
            services.AddSingleton<IOperationRepository>(provider =>
                new CachedOperationRepository(provider.GetRequiredService<InMemoryOperationRepository>()));
        }
        else
        {
            services.AddSingleton<IBankAccountRepository, InMemoryBankAccountRepository>();
            services.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();
            services.AddSingleton<IOperationRepository, InMemoryOperationRepository>();
        }

        services.AddSingleton<IAnalyticsService, AnalyticsService>();
        services.AddSingleton<IImportExportService, ImportExportService>();

        services.AddSingleton<IBankAccountFacade, BankAccountFacade>();
        services.AddSingleton<ICategoryFacade, CategoryFacade>();
        services.AddSingleton<IOperationFacade, OperationFacade>();
        services.AddSingleton<IAnalyticsFacade, AnalyticsFacade>();

        return services.BuildServiceProvider();
    }
}