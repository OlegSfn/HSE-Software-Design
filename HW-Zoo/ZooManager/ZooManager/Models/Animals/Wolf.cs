using ZooManager.Models.Abstractions;

namespace ZooManager.Models.Animals;

public class Wolf : Predator
{
    public Wolf(string name, int foodEatenPerDay, int number, bool isHealthy) 
        : base(name, foodEatenPerDay, number, isHealthy)
    {
    }
}