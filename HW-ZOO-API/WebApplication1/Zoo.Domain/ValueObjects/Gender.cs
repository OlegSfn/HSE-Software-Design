namespace Zoo.Domain.ValueObjects
{
    public class Gender
    {
        public static readonly Gender Male = new("Male");
        public static readonly Gender Female = new("Female");
        
        private readonly string _value;
        
        private Gender(string value)
        {
            _value = value;
        }
        
        public static Gender FromString(string value)
        {
            return value.ToLower() switch
            {
                "male" => Male,
                "female" => Female,
                _ => throw new ArgumentException($"Invalid gender: {value}")
            };
        }
        
        public override string ToString()
        {
            return _value;
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is Gender gender)
            {
                return _value == gender._value;
            }
            
            return false;
        }
        
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}