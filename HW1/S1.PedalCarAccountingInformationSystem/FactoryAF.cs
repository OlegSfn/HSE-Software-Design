namespace S1.PedalCarAccountingInformationSystem;

public class FactoryAF
{
    public List<Car> Cars { get; }

    public List<Customer> Customers { get; private set; }

    public FactoryAF(List<Customer> customers)
    {
        Customers = customers;

        Cars = new List<Car>();
    }

    internal void SaleCar()
    {
        foreach (Customer customer in Customers)
        {
            customer.Car ??= Cars.LastOrDefault();

            if (customer.Car == null || Cars.Count == 0)
                break;

            Cars.RemoveAt(Cars.Count - 1);
        }

        Customers = Customers.Where(customer => customer.Car == null).ToList();
        Cars.Clear();
    }

    internal void AddCar()
    {
        Car car = new() { Number = Cars.Count + 1 };
        Cars.Add(car);
    }
}
