using S2.HseCarShop.Models.Abstractions;

namespace S2.HseCarShop.Models;

/// <summary>
/// Ручной двигатель
/// </summary>
public class HandEngine : IEngine
{
    public EngineType Type => EngineType.Hand;
    
    // Переопределим метод ToString для получения информации о двигателе
    public override string ToString() => $"Тип: {Type}";
    
    public bool IsSuitableForCustomer(Customer customer)
    {
        return customer.HandStrength > 5;
    }
}