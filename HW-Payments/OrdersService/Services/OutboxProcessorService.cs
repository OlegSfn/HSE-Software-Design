using Newtonsoft.Json;
using Shared.Messaging.Infrastructure;
using Shared.Messaging.Infrastructure.MessageBroker;
using Shared.Messaging.Infrastructure.Outbox;
using Shared.Messaging.Messages;

namespace OrdersService.Services;

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
            case "PaymentRequest":
                var paymentRequest = JsonConvert.DeserializeObject<PaymentRequestMessage>(message.Content);
                await messageBroker.PublishAsync("payment-requests", paymentRequest);
                break;

            default:
                Logger.LogWarning("Unknown message type: {Type}", message.Type);
                break;
        }
    }
}