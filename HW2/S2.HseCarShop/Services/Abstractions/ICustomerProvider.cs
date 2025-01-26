using S2.HseCarShop.Models;

namespace S2.HseCarShop.Services.Abstractions;

/// <summary>
/// Интерфейс, предоставляющий список покупателей
/// </summary>
public interface ICustomersProvider
{
    /// <summary>
    /// Возвращает покупателей в очереди за машиной
    /// </summary>
    IEnumerable<Customer> GetCustomers();
}
