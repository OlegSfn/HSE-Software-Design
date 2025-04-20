namespace Zoo.Domain.ValueObjects
{
    public class FoodType
    {
        public static readonly FoodType Meat = new("Meat");
        public static readonly FoodType Fruits = new("Fruits");
        public static readonly FoodType Seeds = new("Seeds");
        
        private readonly string _value;
        
        private FoodType(string value)
        {
            _value = value;
        }
        
        public static FoodType FromString(string value)
        {
            return value.ToLower() switch
            {
                "meat" => Meat,
                "fruits" => Fruits,
                "seeds" => Seeds,
                _ => throw new ArgumentException($"Invalid food type: {value}")
            };
        }
        
        public override string ToString()
        {
            return _value;
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is FoodType type)
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