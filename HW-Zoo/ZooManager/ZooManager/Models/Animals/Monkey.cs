using ZooManager.Models.Abstractions;

namespace ZooManager.Models.Animals;

public class Monkey : Herbo
{
    public Monkey(string name, int foodEatenPerDay, int number, bool isHealthy, int kindnessLevel) 
        : base(name, foodEatenPerDay, number, isHealthy, kindnessLevel)
    {
    }
}