using Zoo.Domain.Events;

namespace Zoo.Application.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T eventData) where T : class;
        Task PublishAnimalMovedEventAsync(AnimalMovedEvent eventData);
        Task PublishFeedingTimeEventAsync(FeedingTimeEvent eventData);
    }
}