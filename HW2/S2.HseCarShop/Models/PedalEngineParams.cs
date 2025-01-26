using S2.HseCarShop.Models.Abstractions;

namespace S2.HseCarShop.Models;

/// <summary>
/// Параметры педального двигателя
/// </summary>
public record PedalEngineParams : EngineParamsBase
{
    /// <summary>
    /// Размер педалей
    /// </summary>
    public uint PedalSize { get; }

    public PedalEngineParams(uint pedalSize)
    {
        PedalSize = pedalSize;
    }
}