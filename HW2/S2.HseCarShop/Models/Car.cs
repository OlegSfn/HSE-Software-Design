using S2.HseCarShop.Models.Abstractions;

namespace S2.HseCarShop.Models;

/// <summary>
/// Автомобиль
/// </summary>
public class Car
{
    /// <summary>
    /// Номер автомобиля
    /// </summary>
    public Guid Number { get; }

    /// <summary>
    /// Двигатель автомобиля
    /// </summary>
    public IEngine Engine { get; }

    public Car(IEngine engine, Guid number)
    {
        ArgumentNullException.ThrowIfNull(engine, nameof(engine));

        Engine = engine;
        Number = number;
    }

    // Переопределим метод ToString для получения информации об автомобиле
    public override string ToString()
        => $"Номер: {Number}, Двигатель: {{ {Engine} }}";
    
    /// <summary>
    /// Проверка подходит ли автомобиль для покупателя
    /// </summary>
    /// <param name="customer">Покупатель для проверки</param>
    /// <returns>true, если автомобиль подходит переданному покупателю, иначе false</returns>
    public bool IsSuitableForCustomer(Customer customer)
    {
        return Engine.IsSuitableForCustomer(customer);
    }
}
