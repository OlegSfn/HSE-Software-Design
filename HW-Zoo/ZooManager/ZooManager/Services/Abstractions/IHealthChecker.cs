using ZooManager.Models.Abstractions;

namespace ZooManager.Services.Abstractions;

public interface IHealthChecker
{
    public bool CheckHealth(Animal animal);
}