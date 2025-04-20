using Zoo.Domain.ValueObjects;

namespace Zoo.Domain.Entities
{
    public class FeedingSchedule
    {
        public Guid Id { get; private set; }
        public Guid AnimalId { get; private set; }
        public DateTime FeedingTime { get; private set; }
        public FoodType FoodType { get; private set; }
        public bool IsCompleted { get; private set; }
        public DateTime? CompletionTime { get; private set; }
        
        public FeedingSchedule(Guid animalId, DateTime feedingTime, FoodType foodType)
        {
            Id = Guid.NewGuid();
            AnimalId = animalId != Guid.Empty ? animalId : throw new ArgumentException("Animal ID cannot be empty", nameof(animalId));
            FeedingTime = feedingTime;
            FoodType = foodType ?? throw new ArgumentNullException(nameof(foodType));
            IsCompleted = false;
            CompletionTime = null;
        }
        
        public void MarkAsCompleted()
        {
            IsCompleted = true;
            CompletionTime = DateTime.Now;
        }
        
        public void UpdateSchedule(DateTime newFeedingTime, FoodType newFoodType)
        {
            FeedingTime = newFeedingTime;
            FoodType = newFoodType ?? throw new ArgumentNullException(nameof(newFoodType));
        }
    }
}