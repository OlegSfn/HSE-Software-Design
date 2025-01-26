using S2.HseCarShop.Models.Abstractions;

namespace S2.HseCarShop.Models;

/// <summary>
/// Структура для случая, когда у двигателя нет каких-либо параметров
/// </summary>
public record EmptyEngineParams : EngineParamsBase
{
    /// <summary>
    /// Параметры двигателя по умолчанию
    /// </summary>
    public static EmptyEngineParams DEFAULT { get; } = new EmptyEngineParams();
}
