using ZooManager.Models.Things;
using Xunit;

namespace ZooManagerTests.ModelsTests.ThingsTests;

public class ThingsConstructorsTests
{
    [Fact]
    public void ComputerConstructor_ShouldSetProperties()
    {
        Computer computer = new Computer(52);
        
        Assert.Equal(52, computer.Number);
    }
    
    [Fact]
    public void TableConstructor_ShouldSetProperties()
    {
        Table table = new Table(52);
        
        Assert.Equal(52, table.Number);
    }
}