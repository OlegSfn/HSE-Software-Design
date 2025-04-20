namespace Zoo.Presentation.DTOs
{
    /// <summary>
    /// Statistical information about the current state of the zoo
    /// </summary>
    public class ZooStatisticsDTO
    {
        /// <summary>
        /// Total number of animals in the zoo
        /// </summary>
        /// <example>52</example>
        public int TotalAnimals { get; set; }
        
        /// <summary>
        /// Number of animals currently in healthy status
        /// </summary>
        /// <example>42</example>
        public int HealthyAnimals { get; set; }
        
        /// <summary>
        /// Number of animals currently in sick status
        /// </summary>
        /// <example>0</example>
        public int SickAnimals { get; set; }
        
        /// <summary>
        /// Total number of enclosures in the zoo
        /// </summary>
        /// <example>100</example>
        public int TotalEnclosures { get; set; }
        
        /// <summary>
        /// Number of enclosures that have space for more animals
        /// </summary>
        /// <example>5</example>
        public int AvailableEnclosures { get; set; }
        
        /// <summary>
        /// Number of enclosures that are at maximum capacity
        /// </summary>
        /// <example>2</example>
        public int FullEnclosures { get; set; }
        
        /// <summary>
        /// Distribution of enclosures by type
        /// </summary>
        /// <example>{"Predator": 5, "Herbivore": 2, "Bird": 5, "Aquarium": 2}</example>
        public Dictionary<string, int> EnclosuresByType { get; set; }
        
        /// <summary>
        /// Distribution of animals by species/type
        /// </summary>
        /// <example>{"Predator": 52, "Herbivore": 25, "Bird": 520, "Aquatic": 5252}</example>
        public Dictionary<string, int> AnimalsBySpecies { get; set; }
        
        /// <summary>
        /// Number of scheduled feedings that have not been completed
        /// </summary>
        /// <example>5</example>
        public int PendingFeedings { get; set; }
        
        /// <summary>
        /// Number of scheduled feedings that have been completed
        /// </summary>
        /// <example>1</example>
        public int CompletedFeedings { get; set; }
        
        /// <summary>
        /// Number of feedings scheduled for today
        /// </summary>
        /// <example>2</example>
        public int FeedingsToday { get; set; }
        
        /// <summary>
        /// Distribution of animals by gender
        /// </summary>
        /// <example>{"Male": 22, "Female": 22}</example>
        public Dictionary<string, int> GenderDistribution { get; set; }
    }
}