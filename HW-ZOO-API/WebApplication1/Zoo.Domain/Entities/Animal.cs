using Zoo.Domain.ValueObjects;

namespace Zoo.Domain.Entities
{
    public class Animal
    {
        public Guid Id { get; private set; }
        public AnimalType Type { get; private set; }
        public Name Name { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public Gender Gender { get; private set; }
        public FoodType FavoriteFood { get; private set; }
        public HealthStatus HealthStatus { get; private set; }
        public Guid? EnclosureId { get; private set; }
        public DateTime? LastFeedingTime { get; private set; }
        
        public Animal(AnimalType type, Name name, DateTime dateOfBirth, Gender gender, FoodType favoriteFood)
        {
            Id = Guid.NewGuid();
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Name = name ?? throw new ArgumentException(nameof(name));
            DateOfBirth = dateOfBirth;
            Gender = gender ?? throw new ArgumentNullException(nameof(gender));
            FavoriteFood = favoriteFood ?? throw new ArgumentNullException(nameof(favoriteFood));
            HealthStatus = HealthStatus.Healthy;
            EnclosureId = null;
            LastFeedingTime = null;
        }
        
        public void Feed()
        {
            LastFeedingTime = DateTime.Now;
        }
        
        public void Treat()
        {
            HealthStatus = HealthStatus.Healthy;
        }
        
        public void MakeSick()
        {
            HealthStatus = HealthStatus.Sick;
        }
        
        public void AssignToEnclosure(Guid enclosureId)
        {
            if (enclosureId == Guid.Empty)
                throw new ArgumentException("Enclosure ID cannot be empty", nameof(enclosureId));
            
            EnclosureId = enclosureId;
        }
        
        public void RemoveFromEnclosure()
        {
            EnclosureId = null;
        }
        
        public bool IsAssignedToEnclosure()
        {
            return EnclosureId.HasValue;
        }
        
        public bool IsPredator()
        {
            return Type.IsPredator;
        }
        
        public bool IsHerbivore()
        {
            return !Type.IsPredator;
        }
    }
}