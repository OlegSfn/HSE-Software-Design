using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Shared.Messaging.Infrastructure.MessageBroker;
 
public class RabbitMqMessageBroker : IMessageBroker, IDisposable
{
    private IConnection? _connection;
    private IModel? _channel;
    private readonly ILogger<RabbitMqMessageBroker> _logger;
    private readonly Dictionary<string, List<object>> _consumerTags = new();
    private bool _isConnected;
    private readonly string _hostName;
    private readonly int _maxRetryAttempts = 5;
 
    public RabbitMqMessageBroker(ILogger<RabbitMqMessageBroker> logger, string hostName = "localhost")
    {
        _logger = logger;
        _hostName = hostName;
        _isConnected = false;
 
        ConnectWithRetry();
    }
 
    private void ConnectWithRetry()
    {
        var retryCount = 0;
        var backoffTime = TimeSpan.FromSeconds(2);
 
        while (retryCount < _maxRetryAttempts && !_isConnected)
        {
            try
            {
                _logger.LogInformation("Attempting to connect to RabbitMQ at {HostName}, attempt {RetryCount}/{MaxRetries}",
                     _hostName, retryCount + 1, _maxRetryAttempts);
 
                var factory = new ConnectionFactory { HostName = _hostName };
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _isConnected = true;
 
                _logger.LogInformation("Successfully connected to RabbitMQ");
            }
            catch (Exception ex)
            {
                ++retryCount;
                if (retryCount >= _maxRetryAttempts)
                {
                    _logger.LogError(ex, "Failed to connect to RabbitMQ after {MaxRetries} attempts. Message broker functionality will be limited.",
                        _maxRetryAttempts);
                    break;
                }
 
                _logger.LogWarning(ex, "Failed to connect to RabbitMQ, retrying in {BackoffTime}s (attempt {RetryCount}/{MaxRetries})",
                    backoffTime.TotalSeconds, retryCount, _maxRetryAttempts);
 
                Thread.Sleep(backoffTime);
                backoffTime = TimeSpan.FromSeconds(Math.Min(backoffTime.TotalSeconds * 2, 30));
            }
        }
    }
 
    public async Task PublishAsync<T>(string queue, T message)
    {
        if (!_isConnected || _channel == null)
        {
            _logger.LogWarning("Cannot publish message to queue {Queue}: RabbitMQ connection not available", queue);
            return;
        }
 
        try
        {
            _channel.QueueDeclare(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
 
            var messageJson = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(messageJson);
 
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
 
            _channel.BasicPublish(
                exchange: "",
                routingKey: queue,
                basicProperties: properties,
                body: body
            );
 
            _logger.LogInformation("Published message to queue {Queue}: {Message}", queue, messageJson);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message to queue {Queue}", queue);
        }
 
        await Task.CompletedTask;
    }
 
    public Task SubscribeAsync<T>(string queue, Func<T, Task> handler)
    {
        if (!_isConnected || _channel == null)
        {
            _logger.LogWarning("Cannot subscribe to queue {Queue}: RabbitMQ connection not available", queue);
            return Task.CompletedTask;
        }
 
        try
        {
            _channel.QueueDeclare(
                queue: queue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
 
            var consumer = new EventingBasicConsumer(_channel);
 
            consumer.Received += async (_, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var messageJson = Encoding.UTF8.GetString(body);
                    var message = JsonConvert.DeserializeObject<T>(messageJson);
 
                    _logger.LogInformation("Received message from queue {Queue}: {Message}", queue, messageJson);
 
                    if (message != null)
                    {
                        await handler(message);
                    }
 
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message from queue {Queue}", queue);
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };
 
            _channel.BasicQos(0, 1, false);
 
            var consumerTag = _channel.BasicConsume(queue, false, consumer);
 
            if (!_consumerTags.ContainsKey(queue))
            {
                _consumerTags[queue] = new List<object>();
            }
 
            _consumerTags[queue].Add(consumerTag);
 
            _logger.LogInformation("Successfully subscribed to queue {Queue}", queue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error subscribing to queue {Queue}", queue);
        }
 
        return Task.CompletedTask;
    }
 
    public void Dispose()
    {
        try
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing RabbitMQ resources");
        }
    }
}