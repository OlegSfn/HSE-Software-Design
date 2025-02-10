using ZooManager.Models.Animals;
using ZooManager.Services;
using Xunit;

namespace ZooManagerTests.ServicesTests;

public class VeterinaryClinicTests
{
    [Fact]
    public void CheckHealth_HealthyAnimal_ReturnsTrue()
    {
        var clinic = new VeterinaryClinic();
        var animal = new Rabbit("", 0, 0, true, 0);

        var result = clinic.CheckHealth(animal);
            
        Assert.True(result);
    }

    [Fact]
    public void CheckHealth_UnhealthyAnimal_ReturnsFalse()
    {
        var clinic = new VeterinaryClinic();
        var animal = new Rabbit("", 0, 0, false, 0);

        var result = clinic.CheckHealth(animal);
            
        Assert.False(result);
    }
}