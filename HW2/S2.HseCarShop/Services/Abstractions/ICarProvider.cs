using S2.HseCarShop.Models;

namespace S2.HseCarShop.Services.Abstractions;

/// <summary>
/// Интерфейс, предоставляющий автомобили для покупателей
/// </summary>
public interface ICarProvider
{
    /// <summary>
    /// Получить автомобиль для покупателя
    /// </summary>
    /// <param name="customer">Покупатель, ищущий автомобиль</param>
    /// <returns>Машину, если найдена подходящая или null</returns>
    Car? GetCar(Customer customer);
}
