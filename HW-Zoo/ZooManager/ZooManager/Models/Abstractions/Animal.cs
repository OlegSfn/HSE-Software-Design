namespace ZooManager.Models.Abstractions;

public abstract class Animal : IAlive, IInventory
{
    public int FoodEatenPerDay { get; set; }
    public int Number { get; set; }
    public string Name { get; set; }
    public bool IsHealthy { get; set; }
    
    protected Animal(string name, int foodEatenPerDay, int number, bool isHealthy)
    {
        Name = name;
        FoodEatenPerDay = foodEatenPerDay;
        Number = number;
        IsHealthy = isHealthy;
    }
}