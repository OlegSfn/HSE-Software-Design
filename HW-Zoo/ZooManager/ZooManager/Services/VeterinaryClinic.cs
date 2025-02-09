using ZooManager.Models.Abstractions;
using ZooManager.Services.Abstractions;

namespace ZooManager.Services;

public class VeterinaryClinic : IHealthChecker
{
    public bool CheckHealth(Animal animal)
    {
        return animal.IsHealthy;
    }
}