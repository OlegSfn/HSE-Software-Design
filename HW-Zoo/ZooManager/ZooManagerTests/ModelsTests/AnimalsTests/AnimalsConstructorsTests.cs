using ZooManager.Models.Animals;
using Xunit;

namespace ZooManagerTests.ModelsTests.Animals;

public class AnimalsConstructorsTests
{
    [Fact]
    public void MonkeyConstructor_ShouldSetProperties()
    {
        var monkey = new Monkey("Monkey", 2, 3, true, 4);

        Assert.Equal("Monkey", monkey.Name);
        Assert.Equal(2, monkey.FoodEatenPerDay);
        Assert.Equal(3, monkey.Number);
        Assert.True(monkey.IsHealthy);
        Assert.Equal(4, monkey.KindnessLevel);
    }
    
    [Fact]
    public void RabbitConstructor_ShouldSetProperties()
    {
        var rabbit = new Rabbit("Rabbit", 1, 2, true, 5);

        Assert.Equal("Rabbit", rabbit.Name);
        Assert.Equal(1, rabbit.FoodEatenPerDay);
        Assert.Equal(2, rabbit.Number);
        Assert.True(rabbit.IsHealthy);
        Assert.Equal(5, rabbit.KindnessLevel);
    }
    
    [Fact]
    public void TigerConstructor_ShouldSetProperties()
    {
        var tiger = new Tiger("Tiger", 10, 1, true);

        Assert.Equal("Tiger", tiger.Name);
        Assert.Equal(10, tiger.FoodEatenPerDay);
        Assert.Equal(1, tiger.Number);
        Assert.True(tiger.IsHealthy);
    }
    
    [Fact]
    public void WolfConstructor_ShouldSetProperties()
    {
        var wolf = new Wolf("Wolf", 5, 2, true);

        Assert.Equal("Wolf", wolf.Name);
        Assert.Equal(5, wolf.FoodEatenPerDay);
        Assert.Equal(2, wolf.Number);
        Assert.True(wolf.IsHealthy);
    }
    
    [Fact]
    public void Herbo_Constructor_Must_Throw_ArgumentOutOfRangeException_When_KindnessLevel_Is_Less_Than_Zero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Rabbit("Rabbit", 1, 2, true, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Monkey("Monkey", 1, 2, true, -1));
    }
}