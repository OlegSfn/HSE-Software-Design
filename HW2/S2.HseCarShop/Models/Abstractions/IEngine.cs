namespace S2.HseCarShop.Models.Abstractions;

/// <summary>
/// Интерфейс для описания двигателя
/// </summary>
public interface IEngine
{
    /// <summary>
    /// Тип двигателя
    /// </summary>
    EngineType Type { get; }
    
    /// <summary>
    /// Проверяет, что двигатель подходит для покупателя
    /// </summary>
    /// <param name="customer">Покупатель для проверки</param>
    /// <returns>true, если двигатель подходит переданному покупателю</returns>
    bool IsSuitableForCustomer(Customer customer);
}
