namespace S1.PedalCarAccountingInformationSystem;

internal static class Program
{
    private static void Main()
    {
        List<Customer> customers = new()
        {
            new Customer { Name = "Ivan" },
            new Customer { Name = "Petr" },
            new Customer { Name = "Sidr" }
        };

        FactoryAF factory = new(customers);

        for (int i = 0; i < 5; i++)
            factory.AddCar();

        Console.WriteLine("Before");
        PrintData(factory, customers);

        factory.SaleCar();

        Console.WriteLine("After");
        PrintData(factory, customers);

    }

    private static void PrintData(FactoryAF factory, IEnumerable<Customer> customers)
    {
        Console.WriteLine("Cars:");
        Console.WriteLine(string.Join(Environment.NewLine, factory.Cars));
        Console.WriteLine("Customers:");
        Console.WriteLine(string.Join(Environment.NewLine, customers));
    }
}
