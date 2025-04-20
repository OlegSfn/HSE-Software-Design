using Zoo.Domain.ValueObjects;

namespace Zoo.Domain.Entities
{
    public class Enclosure
    {
        private readonly List<Guid> _animalIds = new();
        
        public Guid Id { get; private set; }
        public EnclosureType Type { get; private set; }
        public NonNegativeInt Size { get; private set; }
        public NonNegativeInt MaxCapacity { get; private set; }
        public NonNegativeInt CurrentAnimalCount => NonNegativeInt.FromInt(_animalIds.Count);
        public DateTime LastCleaned { get; private set; }
        public IEnumerable<Guid> AnimalIds => _animalIds.AsReadOnly();
        
        public Enclosure(EnclosureType type, NonNegativeInt size, NonNegativeInt maxCapacity)
        {
            Id = Guid.NewGuid();
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Size = size ?? throw new ArgumentNullException(nameof(size));
            MaxCapacity = maxCapacity ?? throw new ArgumentNullException(nameof(maxCapacity));
            LastCleaned = DateTime.Now;
        }
        
        public bool AddAnimal(Guid animalId)
        {
            if (animalId == Guid.Empty)
            {
                throw new ArgumentException("Animal ID cannot be empty", nameof(animalId));
            }

            if (!IsAvailable() || _animalIds.Contains(animalId))
            {
                return false;
            }
                
            _animalIds.Add(animalId);
            return true;
        }
        
        public bool RemoveAnimal(Guid animalId)
        {
            if (animalId == Guid.Empty)
            {
                throw new ArgumentException("Animal ID cannot be empty", nameof(animalId));
            }
                
            return _animalIds.Remove(animalId);
        }
        
        public void Clean()
        {
            LastCleaned = DateTime.Now;
        }
        
        public bool IsAvailable()
        {
            return CurrentAnimalCount < MaxCapacity;
        }
        
        public bool IsCompatibleWithAnimal(Animal animal)
        {
            if (animal == null)
            {
                throw new ArgumentNullException(nameof(animal));
            }

            if (!IsAvailable())
            {
                return false;
            }
                
            return Type switch
            {
                var t when t == EnclosureType.Predator => animal.IsPredator(),
                var t when t == EnclosureType.Herbivore => animal.IsHerbivore(),
                var t when t == EnclosureType.Bird => animal.Type.Name == "Bird",
                var t when t == EnclosureType.Aquarium => animal.Type.Name == "Aquatic",
                _ => false
            };
        }
    }
}