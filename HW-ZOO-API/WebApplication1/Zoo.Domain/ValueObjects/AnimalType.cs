namespace Zoo.Domain.ValueObjects
{
    public class AnimalType
    {
        public string Name { get; }
        public bool IsPredator { get; }

        private AnimalType(string name, bool isPredator)
        {
            Name = name;
            IsPredator = isPredator;
        }

        public static AnimalType Predator => new("Predator", true);
        public static AnimalType Herbivore => new("Herbivore", false);
        public static AnimalType Bird => new("Bird", false);
        public static AnimalType Aquatic => new("Aquatic", false);

        public static AnimalType FromString(string typeString)
        {
            if (string.IsNullOrWhiteSpace(typeString))
                throw new ArgumentException("Animal type cannot be empty", nameof(typeString));

            return typeString.ToLowerInvariant() switch
            {
                "predator" => Predator,
                "herbivore" => Herbivore,
                "bird" => Bird,
                "aquatic" => Aquatic,
                _ => throw new ArgumentException($"Unknown animal type: {typeString}", nameof(typeString))
            };
        }

        public override string ToString() => Name;

        public override bool Equals(object? obj)
        {
            if (obj is AnimalType other)
            {
                return Name == other.Name && IsPredator == other.IsPredator;
            }
            
            return false;
        }

        public override int GetHashCode() => HashCode.Combine(Name, IsPredator);
    }
}