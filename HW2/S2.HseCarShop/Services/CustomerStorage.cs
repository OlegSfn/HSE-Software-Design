using S2.HseCarShop.Models;
using S2.HseCarShop.Services.Abstractions;

namespace S2.HseCarShop.Services;

/// <summary>
/// Хранилище покупателей
/// </summary>
public class CustomerStorage : ICustomersProvider
{
    /// <summary>
    /// Список покупателей
    /// </summary>
    public List<Customer> Customers { get; }

    public CustomerStorage()
    {
        Customers = [];
    }

    /// <summary>
    /// Добавление покупателя
    /// </summary>
    /// <param name="customer">Покупатель</param>
    public void AddCustomer(Customer customer)
    {
        ArgumentNullException.ThrowIfNull(customer, nameof(customer));

        Customers.Add(customer);
    }

    public IEnumerable<Customer> GetCustomers()
        => Customers.Where(customer => customer.Car == null);
}