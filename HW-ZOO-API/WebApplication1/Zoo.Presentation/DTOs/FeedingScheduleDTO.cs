namespace Zoo.Presentation.DTOs
{
    /// <summary>
    /// Represents a feeding schedule entry in the zoo system
    /// </summary>
    public class FeedingScheduleDTO
    {
        /// <summary>
        /// Unique identifier for the feeding schedule entry
        /// </summary>
        /// <example>77cfc07c-6fe0-45c9-8a25-bbe6bced185e</example>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Identifier of the animal to be fed
        /// </summary>
        /// <example>76b03f33-3850-4d9a-ab75-1fafd63f850b</example>
        public Guid AnimalId { get; set; }
        
        /// <summary>
        /// Name of the animal to be fed
        /// </summary>
        /// <example>Businka</example>
        public string AnimalName { get; set; }
        
        /// <summary>
        /// The time of day when feeding should occur
        /// </summary>
        /// <example>2025-04-13T13:00:00</example>
        public DateTime FeedingTime { get; set; }
        
        /// <summary>
        /// Type of food to be provided
        /// </summary>
        /// <example>Meat</example>
        public string FoodType { get; set; }
        
        /// <summary>
        /// Indicates whether the feeding has been completed
        /// </summary>
        /// <example>false</example>
        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// Data required to create a new feeding schedule entry
    /// </summary>
    public class CreateFeedingScheduleDTO
    {
        /// <summary>
        /// Identifier of the animal to be fed
        /// </summary>
        /// <example>76b03f33-3850-4d9a-ab75-1fafd63f850b</example>
        public Guid AnimalId { get; set; }
        
        /// <summary>
        /// The time of day when feeding should occur
        /// </summary>
        /// <example>2025-04-13T13:00:00</example>
        public DateTime FeedingTime { get; set; }
        
        /// <summary>
        /// Type of food to be provided
        /// </summary>
        /// <example>Meat</example>
        public string FoodType { get; set; }
    }

    /// <summary>
    /// Data required to update an existing feeding schedule entry
    /// </summary>
    public class UpdateFeedingScheduleDTO
    {
        /// <summary>
        /// The new time of day when feeding should occur
        /// </summary>
        /// <example>2025-04-13T13:00:00</example>
        public DateTime FeedingTime { get; set; }
        
        /// <summary>
        /// The new type of food to be provided
        /// </summary>
        /// <example>Meat</example>
        public string FoodType { get; set; }
    }
}