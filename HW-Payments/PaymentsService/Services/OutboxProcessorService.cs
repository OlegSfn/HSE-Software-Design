using Newtonsoft.Json;
using PaymentsService.Repositories;
using Shared.Messaging.Infrastructure;
using Shared.Messaging.Infrastructure.MessageBroker;
using Shared.Messaging.Infrastructure.Outbox;
using Shared.Messaging.Messages;

namespace PaymentsService.Services;

public class OutboxProcessorService : BaseOutboxProcessorService
{
    public OutboxProcessorService(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxProcessorService> logger,
        IConfiguration configuration)
        : base(scopeFactory, logger, configuration)
    {
    }

    protected override async Task HandleMessage(OutboxMessage message)
    {
        using var scope = ScopeFactory.CreateScope();
        var messageBroker = scope.ServiceProvider.GetRequiredService<IMessageBroker>();
        
        switch (message.Type)
        {
            case "PaymentStatus":
                var paymentStatus = JsonConvert.DeserializeObject<PaymentStatusMessage>(message.Content);
                await messageBroker.PublishAsync("payment-statuses", paymentStatus);
                break;

            default:
                Logger.LogWarning("Unknown message type: {Type}", message.Type);
                break;
        }
    }
}