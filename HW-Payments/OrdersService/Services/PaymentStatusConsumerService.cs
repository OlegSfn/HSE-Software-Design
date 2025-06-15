using Shared.Messaging.Infrastructure;
using Shared.Messaging.Infrastructure.MessageBroker;
using Shared.Messaging.Messages;

namespace OrdersService.Services;

public class PaymentStatusConsumerService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PaymentStatusConsumerService> _logger;
    private readonly IMessageBroker _messageBroker;
    
    private const string PaymentStatusesTopic = "payment-statuses";

    public PaymentStatusConsumerService(IServiceScopeFactory scopeFactory, ILogger<PaymentStatusConsumerService> logger, IMessageBroker messageBroker)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _messageBroker = messageBroker;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Payment Status Consumer Service is starting");

        await _messageBroker.SubscribeAsync<PaymentStatusMessage>(PaymentStatusesTopic, ProcessPaymentStatusMessageAsync);

        await Task.Delay(Timeout.Infinite, stoppingToken);

        _logger.LogInformation("Payment Status Consumer Service is stopping");
    }

    private async Task ProcessPaymentStatusMessageAsync(PaymentStatusMessage message)
    {
        _logger.LogInformation("Processing payment status message for order {OrderId}, Status: {Status}, Success: {Success}, ErrorMessage: {ErrorMessage}",
            message.OrderId, message.Status, message.Success, message.ErrorMessage);

        using var scope = _scopeFactory.CreateScope();
        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

        bool success = message.Status == "Completed";
        string reason = message.ErrorMessage ?? "Unknown error";

        _logger.LogInformation("Updating order {OrderId} with success={Success}, reason={Reason}",
            message.OrderId, success, reason);

        await orderService.HandlePaymentStatusAsync(message.OrderId, success, reason);
    }
}