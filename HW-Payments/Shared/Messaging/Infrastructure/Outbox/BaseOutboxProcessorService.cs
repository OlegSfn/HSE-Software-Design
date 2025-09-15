using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Shared.Messaging.Infrastructure.Outbox;

public abstract class BaseOutboxProcessorService : BackgroundService
{
    protected readonly IServiceScopeFactory ScopeFactory;
    protected readonly ILogger<BaseOutboxProcessorService> Logger;
    private readonly IConfiguration _configuration;
    private readonly TimeSpan _pollingInterval;

    public BaseOutboxProcessorService(IServiceScopeFactory scopeFactory, ILogger<BaseOutboxProcessorService> logger, IConfiguration configuration)
    {
        ScopeFactory = scopeFactory;
        Logger = logger;
        _configuration = configuration;

        _pollingInterval = TimeSpan.FromSeconds(_configuration.GetValue<double>("OutboxProcessor:PollingIntervalSeconds", 5));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Logger.LogInformation("Outbox Processor Service is starting");

        while(!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxMessagesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error occurred while processing outbox messages");
            }

            await Task.Delay(_pollingInterval, stoppingToken);
        }

        Logger.LogInformation("Outbox Processor Service is stopping");
    }

    private async Task ProcessOutboxMessagesAsync(CancellationToken stoppingToken)
    {
        using var scope = ScopeFactory.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();

        var messages = await outboxRepository.GetPendingOutboxMessagesAsync();

        if (!messages.Any())
        {
            return;
        }

        Logger.LogInformation("Found {Count} pending outbox messages to process", messages.Count());

        foreach (var message in messages)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                await HandleMessage(message);
                await outboxRepository.MarkOutboxMessageAsProcessedAsync(message.Id);
                Logger.LogInformation("Successfully processed outbox message {MessageId}", message.Id);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error processing outbox message {MessageId}", message.Id);
                await outboxRepository.MarkOutboxMessageAsFailedAsync(message.Id, ex.Message);
            }
        }
    }
    
    protected abstract Task HandleMessage(OutboxMessage message);
}