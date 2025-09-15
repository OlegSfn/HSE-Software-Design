using HSEBank.FinancialAccounting.DI;
using HSEBank.FinancialAccounting.Facades;
using HSEBank.FinancialAccounting.Factories;
using HSEBank.FinancialAccounting.Interfaces.Facades;
using HSEBank.FinancialAccounting.Interfaces.Factories;
using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Interfaces.Services;
using HSEBank.FinancialAccounting.Proxies;
using HSEBank.FinancialAccounting.Repositories;
using HSEBank.FinancialAccounting.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.DI;

public class DependencyInjectionTests
{
    [Fact]
    public void ConfigureServices_WithCaching_RegistersAllServices()
    {
        // Act
        var serviceProvider = DependencyInjection.ConfigureServices();

        // Assert
        Assert.NotNull(serviceProvider.GetService<IBankAccountFactory>());
        Assert.NotNull(serviceProvider.GetService<ICategoryFactory>());
        Assert.NotNull(serviceProvider.GetService<IOperationFactory>());

        Assert.NotNull(serviceProvider.GetService<IBankAccountRepository>());
        Assert.NotNull(serviceProvider.GetService<ICategoryRepository>());
        Assert.NotNull(serviceProvider.GetService<IOperationRepository>());

        Assert.NotNull(serviceProvider.GetService<IAnalyticsService>());
        Assert.NotNull(serviceProvider.GetService<IImportExportService>());

        Assert.NotNull(serviceProvider.GetService<IBankAccountFacade>());
        Assert.NotNull(serviceProvider.GetService<ICategoryFacade>());
        Assert.NotNull(serviceProvider.GetService<IOperationFacade>());
        Assert.NotNull(serviceProvider.GetService<IAnalyticsFacade>());
    }

    [Fact]
    public void ConfigureServices_WithCaching_RegistersCachedRepositories()
    {
        // Act
        var serviceProvider = DependencyInjection.ConfigureServices();

        // Assert
        var bankAccountRepository = serviceProvider.GetService<IBankAccountRepository>();
        var categoryRepository = serviceProvider.GetService<ICategoryRepository>();
        var operationRepository = serviceProvider.GetService<IOperationRepository>();

        Assert.IsType<CachedBankAccountRepository>(bankAccountRepository);
        Assert.IsType<CachedCategoryRepository>(categoryRepository);
        Assert.IsType<CachedOperationRepository>(operationRepository);
    }

    [Fact]
    public void ConfigureServices_WithoutCaching_RegistersInMemoryRepositories()
    {
        // Act
        var serviceProvider = DependencyInjection.ConfigureServices(false);

        // Assert
        var bankAccountRepository = serviceProvider.GetService<IBankAccountRepository>();
        var categoryRepository = serviceProvider.GetService<ICategoryRepository>();
        var operationRepository = serviceProvider.GetService<IOperationRepository>();

        Assert.IsType<InMemoryBankAccountRepository>(bankAccountRepository);
        Assert.IsType<InMemoryCategoryRepository>(categoryRepository);
        Assert.IsType<InMemoryOperationRepository>(operationRepository);
    }

    [Fact]
    public void ConfigureServices_RegistersAnalyticsService()
    {
        // Act
        var serviceProvider = DependencyInjection.ConfigureServices();

        // Assert
        var analyticsService = serviceProvider.GetService<IAnalyticsService>();
        Assert.IsType<AnalyticsService>(analyticsService);
    }

    [Fact]
    public void ConfigureServices_RegistersImportExportService()
    {
        // Act
        var serviceProvider = DependencyInjection.ConfigureServices();

        // Assert
        var importExportService = serviceProvider.GetService<IImportExportService>();
        Assert.IsType<ImportExportService>(importExportService);
    }

    [Fact]
    public void ConfigureServices_RegistersFacades()
    {
        // Act
        var serviceProvider = DependencyInjection.ConfigureServices();

        // Assert
        var bankAccountFacade = serviceProvider.GetService<IBankAccountFacade>();
        var categoryFacade = serviceProvider.GetService<ICategoryFacade>();
        var operationFacade = serviceProvider.GetService<IOperationFacade>();
        var analyticsFacade = serviceProvider.GetService<IAnalyticsFacade>();
        Assert.IsType<BankAccountFacade>(bankAccountFacade);
        Assert.IsType<CategoryFacade>(categoryFacade);
        Assert.IsType<OperationFacade>(operationFacade);
        Assert.IsType<AnalyticsFacade>(analyticsFacade);
    }

    [Fact]
    public void ConfigureServices_RegistersFactories()
    {
        // Act
        var serviceProvider = DependencyInjection.ConfigureServices();

        // Assert
        Assert.IsType<BankAccountFactory>(serviceProvider.GetService<IBankAccountFactory>());
        Assert.IsType<CategoryFactory>(serviceProvider.GetService<ICategoryFactory>());
        Assert.IsType<OperationFactory>(serviceProvider.GetService<IOperationFactory>());
    }
}