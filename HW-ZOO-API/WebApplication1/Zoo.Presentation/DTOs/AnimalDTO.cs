namespace Zoo.Presentation.DTOs
{
    /// <summary>
    /// Represents an animal in the zoo system
    /// </summary>
    public class AnimalDTO
    {
        /// <summary>
        /// Unique id for the animal
        /// </summary>
        /// <example>76b03f33-3850-4d9a-ab75-1fafd63f850b</example>
        public Guid Id { get; set; }
        
        /// <summary>
        /// The species or type of animal
        /// </summary>
        /// <example>Predator</example>
        public string AnimalType { get; set; }
        
        /// <summary>
        /// The name given to the animal
        /// </summary>
        /// <example>Businka</example>
        public string Name { get; set; }
        
        /// <summary>
        /// The animal's date of birth
        /// </summary>
        /// <example>2021-08-30</example>
        public DateTime DateOfBirth { get; set; }
        
        /// <summary>
        /// The gender of the animal
        /// </summary>
        /// <example>Female</example>
        public string Gender { get; set; }
        
        /// <summary>
        /// The animal's preferred food
        /// </summary>
        /// <example>Meat</example>
        public string FavoriteFood { get; set; }
        
        /// <summary>
        /// Current health status of the animal
        /// </summary>
        /// <example>Healthy</example>
        public string HealthStatus { get; set; }
        
        /// <summary>
        /// Identifier of the enclosure where the animal is housed (if any)
        /// </summary>
        /// <example>5717b9c1-60cb-4444-a228-55a5fb7a8490</example>
        public Guid? EnclosureId { get; set; }
    }

    /// <summary>
    /// Data required to create a new animal
    /// </summary>
    public class CreateAnimalDTO
    {
        /// <summary>
        /// The species or type of animal
        /// </summary>
        /// <example>Predator</example>
        public string AnimalType { get; set; }
        
        /// <summary>
        /// The name to assign to the animal
        /// </summary>
        /// <example>Businka</example>
        public string Name { get; set; }
        
        /// <summary>
        /// The animal's date of birth
        /// </summary>
        /// <example>2021-08-30</example>
        public DateTime DateOfBirth { get; set; }
        
        /// <summary>
        /// The gender of the animal
        /// </summary>
        /// <example>Female</example>
        public string Gender { get; set; }
        
        /// <summary>
        /// The animal's preferred food
        /// </summary>
        /// <example>Meat</example>
        public string FavoriteFood { get; set; }
    }

    /// <summary>
    /// Data required to update an animal's health status
    /// </summary>
    public class UpdateAnimalHealthDTO
    {
        /// <summary>
        /// The new health status to set for the animal
        /// </summary>
        /// <example>Sick</example>
        public string HealthStatus { get; set; }
    }
}