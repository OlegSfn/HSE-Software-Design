namespace ZooManager.Models.Abstractions;

public abstract class Herbo : Animal
{
    public int KindnessLevel { get; set; }

    protected Herbo(string name, int foodEatenPerDay, int number, bool isHealthy, int kindnessLevel) 
        : base(name, foodEatenPerDay, number, isHealthy)
    {
        KindnessLevel = kindnessLevel;
    }

}