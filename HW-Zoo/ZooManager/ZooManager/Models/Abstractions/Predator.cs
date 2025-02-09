namespace ZooManager.Models.Abstractions;

public abstract class Predator : Animal
{
    protected Predator(string name, int foodEatenPerDay, int number, bool isHealthy) 
        : base(name, foodEatenPerDay, number, isHealthy)
    {
    }
}