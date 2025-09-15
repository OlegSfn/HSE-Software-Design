using Newtonsoft.Json;
using PaymentsService.Models;
using PaymentsService.Repositories;
using Shared.Messaging.Infrastructure;
using Shared.Messaging.Infrastructure.MessageBroker;
using Shared.Messaging.Messages;

namespace PaymentsService.Services;

public class PaymentRequestConsumerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMessageBroker _messageBroker;
    private readonly ILogger<PaymentRequestConsumerService> _logger;
    
    private const string PaymentRequestsTopic = "payment-requests";

    public PaymentRequestConsumerService(IServiceProvider serviceProvider, IMessageBroker messageBroker, ILogger<PaymentRequestConsumerService> logger)
    {
        _serviceProvider = serviceProvider;
        _messageBroker = messageBroker;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Payment Request Consumer Service is starting");
        await _messageBroker.SubscribeAsync<PaymentRequestMessage>(
            PaymentRequestsTopic,
            async message => await ProcessPaymentRequestAsync(message));
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
        
        _logger.LogInformation("Payment Request Consumer Service is stopping");
    }

    private async Task ProcessPaymentRequestAsync(PaymentRequestMessage message)
    {
        _logger.LogInformation("Processing payment request: {PaymentId} for order {OrderId}",
            message.PaymentId, message.OrderId);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var inboxRepository = scope.ServiceProvider.GetRequiredService<IInboxRepository>();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

            var messageExists = await inboxRepository.HasProcessedMessageAsync(message.PaymentId);
            if (messageExists)
            {
                _logger.LogInformation("Payment request {PaymentId} already processed, skipping", message.PaymentId);
                return;
            }

            var (success, error) = await paymentService.ProcessPaymentRequestAsync(message);

            if (!success)
            {
                _logger.LogError("Failed to process payment request {PaymentId}: {Error}",
                    message.PaymentId, error);
                throw new Exception($"Payment processing failed: {error}");
            }

            var inboxMessage = new InboxMessage
            {
                Id = Guid.NewGuid(),
                MessageId = message.PaymentId,
                Type = "PaymentRequest",
                Content = JsonConvert.SerializeObject(message),
                ReceivedAt = DateTime.UtcNow,
                Status = InboxMessageStatus.Processed,
                ProcessedAt = DateTime.UtcNow
            };

            await inboxRepository.CreateInboxMessageAsync(inboxMessage);

            _logger.LogInformation("Successfully processed payment request {PaymentId}", message.PaymentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment request {PaymentId}", message.PaymentId);
            throw;
        }
    }
}