using System.Text;
using ZooManager.Models.Abstractions;
using ZooManager.Services.Abstractions;

namespace ZooManager.Services;

public class Zoo
{
    private readonly IHealthChecker _clinic;
    public List<Animal> Animals { get; } = new();
    public List<Thing> Things { get; } = new();

    public Zoo(IHealthChecker clinic)
    {
        _clinic = clinic;
    }

    public bool AddAnimal(Animal animal)
    {
        if (!_clinic.CheckHealth(animal))
        {
            return false;
        }
        
        Animals.Add(animal);
        return true;
    }

    public int CalculateTotalFood() => Animals.Sum(a => a.FoodEatenPerDay);

    public List<Herbo> GetContactAnimals() => 
        Animals.OfType<Herbo>().Where(h => h.KindnessLevel > 5).ToList();

    public override string ToString()
    {
        var stringBuilder = new StringBuilder($"Total food needed: {CalculateTotalFood()} kg\n");
        stringBuilder.AppendLine("Contact animals:");
        foreach (var animal in GetContactAnimals())
        {
            stringBuilder.AppendLine($"{animal.Name} ({animal.GetType().Name})");
        }
        
        return stringBuilder.ToString();
    }
}
