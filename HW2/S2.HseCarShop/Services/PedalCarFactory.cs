using S2.HseCarShop.Models;
using S2.HseCarShop.Services.Abstractions;

namespace S2.HseCarShop.Services;

/// <summary>
/// Реализация фабрики для создания педальных автомобилей
/// </summary>
public class PedalCarFactory : ICarFactory<PedalEngineParams>
{
    public Car CreateCar(PedalEngineParams carParams, Guid number)
    {
        var engine = new PedalEngine(carParams.PedalSize);
        return new Car(engine, number);
    }
}
