using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Zoo.Application.Interfaces;

namespace Zoo.Infrastructure
{
    public class EventPublisher : IEventPublisher
    {
        private readonly ILogger<EventPublisher> _logger;

        public EventPublisher(ILogger<EventPublisher> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public Task PublishAsync<T>(T @event) where T : class
        {
            _logger.LogInformation("Event published: {EventType} - {EventData}",
                typeof(T).Name,
                System.Text.Json.JsonSerializer.Serialize(@event));
            
            // In a real implementation, this would publish to an event bus, message broker, etc.
            // For this in-memory implementation, we just log the event
            
            return Task.CompletedTask;
        }
        
        public Task PublishAnimalMovedEventAsync(Domain.Events.AnimalMovedEvent eventData)
        {
            return PublishAsync(eventData);
        }
        
        public Task PublishFeedingTimeEventAsync(Domain.Events.FeedingTimeEvent eventData)
        {
            return PublishAsync(eventData);
        }
    }
}