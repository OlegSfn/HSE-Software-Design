using S2.HseCarShop.Models;
using S2.HseCarShop.Models.Abstractions;

namespace S2.HseCarShop.Services.Abstractions;

/// <summary>
/// Обобщенный интерфейс фабрики для создания автомобилей
/// </summary>
public interface ICarFactory<in TParams>
    where TParams : EngineParamsBase
{
    /// <summary>
    /// Метод для создания автомобиля
    /// </summary>
    /// <param name="carParams">Параметры для создания автомобиля</param>
    /// <param name="number">Номер созданного автомобиля</param>
    /// <returns>Созданный автомобиль</returns>
    Car CreateCar(TParams carParams, Guid number);
}
