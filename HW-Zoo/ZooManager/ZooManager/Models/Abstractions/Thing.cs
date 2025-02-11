namespace ZooManager.Models.Abstractions;

public abstract class Thing : IInventory
{
    public int Number { get; set; }
    
    public Thing(int number)
    {
        Number = number;
    }
}