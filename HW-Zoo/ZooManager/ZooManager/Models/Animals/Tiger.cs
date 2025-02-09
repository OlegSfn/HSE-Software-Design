using ZooManager.Models.Abstractions;

namespace ZooManager.Models.Animals;

public class Tiger : Predator
{
    public Tiger(string name, int foodEatenPerDay, int number, bool isHealthy) 
        : base(name, foodEatenPerDay, number, isHealthy)
    {
    }
}