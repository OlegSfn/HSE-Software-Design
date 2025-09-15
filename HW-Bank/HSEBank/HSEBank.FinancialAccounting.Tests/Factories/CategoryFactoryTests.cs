using HSEBank.FinancialAccounting.Factories;
using HSEBank.FinancialAccounting.Models.Enums;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Factories;

public class CategoryFactoryTests
{
    private readonly CategoryFactory _factory;

    public CategoryFactoryTests()
    {
        _factory = new CategoryFactory();
    }

    [Fact]
    public void Create_WithValidParameters_ReturnsCategory()
    {
        // Arrange
        CategoryType type = CategoryType.Income;
        string name = "Test Category";

        // Act
        var category = _factory.Create(type, name);

        // Assert
        Assert.NotNull(category);
        Assert.Equal(type, category.Type);
        Assert.Equal(name, category.Name);
        Assert.NotEqual(Guid.Empty, category.Id);
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        CategoryType type = CategoryType.Income;
        string name = string.Empty;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _factory.Create(type, name));
        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(CategoryType.Income)]
    [InlineData(CategoryType.Expense)]
    public void Create_WithDifferentTypes_ReturnsCategoryWithCorrectType(CategoryType type)
    {
        // Arrange
        string name = "Test Category";

        // Act
        var category = _factory.Create(type, name);

        // Assert
        Assert.Equal(type, category.Type);
    }

    [Fact]
    public void CreateWithId_WithValidParameters_ReturnsCategory()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        CategoryType type = CategoryType.Income;
        string name = "Test Category";

        // Act
        var category = _factory.CreateWithId(id, type, name);

        // Assert
        Assert.NotNull(category);
        Assert.Equal(id, category.Id);
        Assert.Equal(type, category.Type);
        Assert.Equal(name, category.Name);
    }

    [Fact]
    public void CreateWithId_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        CategoryType type = CategoryType.Income;
        string name = string.Empty;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _factory.CreateWithId(id, type, name));
        Assert.Contains("name", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}