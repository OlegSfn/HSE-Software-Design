using ZooManager.Services.Abstractions;
using ZooManager.Models.Animals;
using ZooManager.Services;
using Xunit;
using Moq;

namespace ZooManagerTests.ServicesTests;

public class ZooTests
{
    private readonly Mock<IHealthChecker> _clinicMock;
    private readonly Zoo _zoo;

    public ZooTests()
    {
        _clinicMock = new Mock<IHealthChecker>();
        _zoo = new Zoo(_clinicMock.Object);
    }

    [Fact]
    public void AddAnimal_HealthyAnimal_AddsToCollection()
    {
        var animal = new Rabbit("", 0, 0, true, 0);
        _clinicMock.Setup(c => c.CheckHealth(animal)).Returns(true);

        var result = _zoo.AddAnimal(animal);
            
        Assert.True(result);
        Assert.Contains(animal, _zoo.Animals);
    }

    [Fact]
    public void AddAnimal_UnhealthyAnimal_DoesNotAdd()
    {
        var animal = new Rabbit("", 0, 0, false, 0);
        _clinicMock.Setup(c => c.CheckHealth(animal)).Returns(false);

        var result = _zoo.AddAnimal(animal);
            
        Assert.False(result);
        Assert.DoesNotContain(animal, _zoo.Animals);
    }

    [Fact]
    public void CalculateTotalFood_WithMultipleAnimals_ReturnsSum()
    {
        _zoo.Animals.Add(new Rabbit("", 2, 0, true, 0));
        _zoo.Animals.Add(new Tiger("", 10, 0, true));

        var result = _zoo.CalculateTotalFood();
            
        Assert.Equal(12, result);
    }

    [Fact]
    public void GetContactAnimals_ReturnsOnlyHerbivoresWithKindnessAbove5()
    {
        var contactRabbit = new Rabbit("", 2, 0, true, 8);
        var nonContactRabbit =new Rabbit("", 2, 0, true, 4);

        _zoo.Animals.AddRange(new[] { contactRabbit, nonContactRabbit });

        var result = _zoo.GetContactAnimals();
            
        Assert.Single(result);
        Assert.Contains(contactRabbit, result);
    }
}