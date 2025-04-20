namespace Zoo.Presentation.DTOs
{
    /// <summary>
    /// Represents an enclosure in the zoo system
    /// </summary>
    public class EnclosureDTO
    {
        /// <summary>
        /// Unique identifier for the enclosure
        /// </summary>
        /// <example>5717b9c1-60cb-4444-a228-55a5fb7a8490</example>
        public Guid Id { get; set; }
        
        /// <summary>
        /// The type of enclosure, which determines what animals it can house
        /// </summary>
        /// <example>Predator</example>
        public string Type { get; set; }
        
        /// <summary>
        /// The size of the enclosure
        /// </summary>
        /// <example>100</example>
        public int Size { get; set; }
        
        /// <summary>
        /// Maximum number of animals the enclosure can fit
        /// </summary>
        /// <example>5</example>
        public int MaxCapacity { get; set; }
        
        /// <summary>
        /// Current number of animals housed in the enclosure
        /// </summary>
        /// <example>3</example>
        public int CurrentAnimalCount { get; set; }
        
        /// <summary>
        /// Date and time when the enclosure was last cleaned
        /// </summary>
        /// <example>2025-04-10T10:30:00</example>
        public DateTime LastCleaned { get; set; }
        
        /// <summary>
        /// List of identifiers for animals currently housed in this enclosure
        /// </summary>
        /// <example>["eae0e875-7cea-45b0-8431-a7b05bd62389", "beb148b0-3334-4ab4-a302-a8263f9e7b19"]</example>
        public IEnumerable<Guid> AnimalIds { get; set; }
    }

    /// <summary>
    /// Data required to create a new enclosure
    /// </summary>
    public class CreateEnclosureDTO
    {
        /// <summary>
        /// The type of enclosure to create
        /// </summary>
        /// <example>Predator</example>
        public string Type { get; set; }
        
        /// <summary>
        /// The size of the enclosure in square meters
        /// </summary>
        /// <example>100</example>
        public int Size { get; set; }
        
        /// <summary>
        /// Maximum number of animals the enclosure can safely house
        /// </summary>
        /// <example>10</example>
        public int MaxCapacity { get; set; }
    }

    /// <summary>
    /// Data for scheduling an enclosure cleaning
    /// </summary>
    public class CleanEnclosureDTO
    {
        /// <summary>
        /// Date and time when the enclosure was or will be cleaned
        /// </summary>
        /// <example>2025-04-13T13:00:00</example>
        public DateTime CleaningTime { get; set; } = DateTime.Now;
    }
}