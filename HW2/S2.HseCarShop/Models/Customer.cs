namespace S2.HseCarShop.Models;

/// <summary>
/// Покупатель
/// </summary>
public class Customer
{
    /// <summary>
    /// Имя покупателя
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Сила ног
    /// </summary>
    public uint HandStrength { get; }

    /// <summary>
    /// Сила рук
    /// </summary>
    public uint LegStrength { get; }

    /// <summary>
    /// Автомобиль
    /// </summary>
    public Car? Car { get; set; }

    public Customer(string name, uint legStrength, uint handStrength)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));

        Name = name;
        LegStrength = legStrength;
        HandStrength = handStrength;
    }

    // Переопределим метод ToString для получения информации о покупателе
    public override string ToString()
    {
        if (Car is null)
            return $"Имя: {Name}, Сила ног: {LegStrength}, Сила рук: {HandStrength}, Автомобиль: Нет";

        return $"Имя: {Name}, Сила ног: {LegStrength}, Сила рук: {HandStrength},\n" +
            $"Автомобиль: {{ {Car} }}";
    }
}
