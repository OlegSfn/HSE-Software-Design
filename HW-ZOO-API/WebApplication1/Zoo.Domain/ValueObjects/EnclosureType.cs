namespace Zoo.Domain.ValueObjects
{
    public class EnclosureType
    {
        public static readonly EnclosureType Predator = new("Predator");
        public static readonly EnclosureType Herbivore = new("Herbivore");
        public static readonly EnclosureType Bird = new("Bird");
        public static readonly EnclosureType Aquarium = new("Aquarium");
        
        private readonly string _value;
        
        private EnclosureType(string value)
        {
            _value = value;
        }
        
        public static EnclosureType FromString(string value)
        {
            return value.ToLower() switch
            {
                "predator" => Predator,
                "herbivore" => Herbivore,
                "bird" => Bird,
                "aquarium" => Aquarium,
                _ => throw new ArgumentException($"Invalid enclosure type: {value}")
            };
        }
        
        public override string ToString()
        {
            return _value;
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is EnclosureType type)
            {
                return _value == type._value;
            }
            
            return false;
        }
        
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}