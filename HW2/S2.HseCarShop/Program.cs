using S2.HseCarShop.Models;
using S2.HseCarShop.Services;

namespace S2.HseCarShop;

internal class Program
{
    static void Main(string[] args)
    {
        var customerStorage = new CustomerStorage();

        var customers = new[] 
        {
            new Customer(name: "Ivan", legStrength: 6, handStrength: 4),
            new Customer(name : "Petr", legStrength : 4, handStrength : 6),
            new Customer(name : "Sidr", legStrength : 6, handStrength : 6),
            new Customer(name : "Sidr", legStrength : 4, handStrength : 4),
        };

        foreach (var customer in customers)
            customerStorage.AddCustomer(customer);

        var pedalCarFactory = new PedalCarFactory();
        var handCarFactory = new HandCarFactory();

        var carService = new CarService();

        carService.AddCar(pedalCarFactory, new PedalEngineParams(pedalSize: 2));
        carService.AddCar(pedalCarFactory, new PedalEngineParams(pedalSize: 3));
        carService.AddCar(handCarFactory, EmptyEngineParams.DEFAULT);
        carService.AddCar(handCarFactory, EmptyEngineParams.DEFAULT);

        var hseCarShop = new HseCarService(carService, customerStorage);

        Console.WriteLine("=== Покупатели ===");
        foreach (var customer in customers)
            Console.WriteLine(customer);

        Console.WriteLine("\n=== Продажа автомобилей ===\n");
        hseCarShop.SellCars();

        Console.WriteLine("=== Покупатели ===");
        foreach (var customer in customers)
            Console.WriteLine(customer);
    }
}
