using S2.HseCarShop.Services.Abstractions;

namespace S2.HseCarShop.Services;

public class HseCarService
{
    /// <summary>
    /// Сервис предоставляющий нам автомобили
    /// </summary>
    private readonly ICarProvider _carProvider;

    /// <summary>
    /// Сервис предоставляющий нам покупателей
    /// </summary>
    private readonly ICustomersProvider _customersProvider;

    public HseCarService(ICarProvider carProvider, ICustomersProvider customersProvider)
    {
        ArgumentNullException.ThrowIfNull(carProvider, nameof(carProvider));
        ArgumentNullException.ThrowIfNull(customersProvider, nameof(customersProvider));

        _carProvider = carProvider;
        _customersProvider = customersProvider;
    }

    /// <summary>
    /// Продаём автомобили всем покупателям в очереди
    /// </summary>
    public void SellCars()
    {
        var customers = _customersProvider.GetCustomers();

        foreach (var customer in customers)
        {
            // Если уже есть авто, то пропускаем
            if (customer.Car != null)
                continue;

            var car = _carProvider.GetCar(customer);

            // Если авто не найдено, то пропускаем
            if (car == null)
                continue;

            customer.Car = car; // иначе вручаем автомобиль
        }
    }
}
