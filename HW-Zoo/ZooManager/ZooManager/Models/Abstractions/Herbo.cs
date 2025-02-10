namespace ZooManager.Models.Abstractions;

public abstract class Herbo : Animal
{
    public int KindnessLevel
    {
        get => _kindnessLevel;
        private init
        {
            if (value < 0 || 10 < value)
            {
                throw new ArgumentOutOfRangeException($"Kindness level must be between 0 and 10, " +
                                                      $"but was {_kindnessLevel}");
            }
            _kindnessLevel = value;
        }
    }
    
    private readonly int _kindnessLevel;

    protected Herbo(string name, int foodEatenPerDay, int number, bool isHealthy, int kindnessLevel) 
        : base(name, foodEatenPerDay, number, isHealthy)
    {
        KindnessLevel = kindnessLevel;
    }

}