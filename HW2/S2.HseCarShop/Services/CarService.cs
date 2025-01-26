using S2.HseCarShop.Models;
using S2.HseCarShop.Models.Abstractions;
using S2.HseCarShop.Services.Abstractions;

namespace S2.HseCarShop.Services;

public class CarService : ICarProvider
{
    /// <summary>
    /// Коллекция для хранения автомобилей
    /// </summary>
    private readonly LinkedList<Car> _cars = new();

    /// <summary>
    /// Методя для добавления автомобиля
    /// </summary>
    public void AddCar<TParams>(ICarFactory<TParams> carFactory, TParams carParams)
        where TParams : EngineParamsBase
    {
        var car = carFactory.CreateCar(carParams, Guid.NewGuid());
        _cars.AddLast(car);
    }

    /// <summary>
    /// Метод для получения автомобиля для покупателя
    /// </summary>
    /// <param name="customer">Покупатель, которому подбирается автомобиль</param>
    /// <returns>Автомобиль, если нашёлся подходящий для покупателя, иначе null</returns>
    public Car? GetCar(Customer customer)
    {
        var car = _cars.FirstOrDefault(car => car.IsSuitableForCustomer(customer));

        if (car != null)
            _cars.Remove(car);

        return car;
    }
}
