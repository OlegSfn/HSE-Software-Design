using Microsoft.Extensions.DependencyInjection;
using ZooManager.Services.Abstractions;
using ZooManager.Models.Animals;
using ZooManager.Services;

namespace ZooManager;

internal static class Program
{
    static void Main()
    {
        var services = new ServiceCollection()
            .AddSingleton<IHealthChecker, VeterinaryClinic>()
            .AddSingleton<Zoo>()
            .BuildServiceProvider();

        var zoo = services.GetRequiredService<Zoo>();
        FillZoo(zoo);
        Console.WriteLine(zoo);
    }

    static void FillZoo(Zoo zoo)
    {
        var tiger = new Tiger("Vova", 13, 1, true);
        var rabbit = new Rabbit("Aboba", 1, 2, true, 7);
        var badRabbit = new Rabbit("Dr. Heinz Doofenshmirtz", 33, 3, true, 0);
        var wolf = new Wolf("Pes", 5, 4, true);
        var monkey = new Monkey("King Kong", 3, 5, false, 10);
        
        zoo.AddAnimal(tiger);
        zoo.AddAnimal(rabbit);
        zoo.AddAnimal(badRabbit);
        zoo.AddAnimal(wolf);
        zoo.AddAnimal(monkey);
    }
}