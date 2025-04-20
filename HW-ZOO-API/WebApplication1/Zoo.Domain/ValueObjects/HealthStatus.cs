namespace Zoo.Domain.ValueObjects
{
    public class HealthStatus
    {
        public static readonly HealthStatus Healthy = new("Healthy");
        public static readonly HealthStatus Sick = new("Sick");
        
        private readonly string _value;
        
        private HealthStatus(string value)
        {
            _value = value;
        }
        
        public static HealthStatus FromString(string value)
        {
            return value.ToLower() switch
            {
                "healthy" => Healthy,
                "sick" => Sick,
                _ => throw new ArgumentException($"Invalid health status: {value}")
            };
        }
        
        public override string ToString()
        {
            return _value;
        }
        
        public override bool Equals(object? obj)
        {
            if (obj is HealthStatus status)
            {
                return _value == status._value;
            }
            
            return false;
        }
        
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }
    }
}