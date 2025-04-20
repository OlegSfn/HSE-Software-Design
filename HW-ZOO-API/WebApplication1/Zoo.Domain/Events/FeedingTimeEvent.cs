using Zoo.Domain.ValueObjects;

namespace Zoo.Domain.Events
{
    public class FeedingTimeEvent
    {
        public Guid AnimalId { get; }
        public Name AnimalName { get; }
        public FoodType FoodType { get; }
        public DateTime Timestamp { get; }
        
        public FeedingTimeEvent(Guid animalId, Name animalName, FoodType foodType)
        {
            AnimalId = animalId;
            AnimalName = animalName;
            FoodType = foodType;
            Timestamp = DateTime.Now;
        }
    }
}