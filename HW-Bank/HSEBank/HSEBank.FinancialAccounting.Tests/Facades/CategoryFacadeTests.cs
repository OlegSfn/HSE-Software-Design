using HSEBank.FinancialAccounting.Facades;
using HSEBank.FinancialAccounting.Factories;
using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;
using Moq;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Facades;

public class CategoryFacadeTests
{
    private readonly Mock<ICategoryRepository> _mockRepository;
    private readonly CategoryFactory _factory;
    private readonly CategoryFacade _facade;
    private readonly Category _testCategory;

    public CategoryFacadeTests()
    {
        _mockRepository = new Mock<ICategoryRepository>();
        _factory = new CategoryFactory();
        _facade = new CategoryFacade(_mockRepository.Object, _factory);
            
        _testCategory = new Category(Guid.NewGuid(), CategoryType.Income, "Test Category");
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CategoryFacade(null, _factory));
    }

    [Fact]
    public void Constructor_WithNullFactory_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CategoryFacade(_mockRepository.Object, null));
    }

    [Fact]
    public void GetAllCategories_CallsRepositoryGetAll()
    {
        // Arrange
        var categories = new[] { _testCategory };
        _mockRepository.Setup(r => r.GetAll()).Returns(categories);

        // Act
        var result = _facade.GetAllCategories();

        // Assert
        Assert.Same(categories, result);
        _mockRepository.Verify(r => r.GetAll(), Times.Once);
    }

    [Fact]
    public void GetCategoryById_CallsRepositoryGetById()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetById(_testCategory.Id)).Returns(_testCategory);

        // Act
        var result = _facade.GetCategoryById(_testCategory.Id);

        // Assert
        Assert.Same(_testCategory, result);
        _mockRepository.Verify(r => r.GetById(_testCategory.Id), Times.Once);
    }

    [Fact]
    public void GetCategoryByName_CallsRepositoryGetByName()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByName(_testCategory.Name)).Returns(_testCategory);

        // Act
        var result = _facade.GetCategoryByName(_testCategory.Name);

        // Assert
        Assert.Same(_testCategory, result);
        _mockRepository.Verify(r => r.GetByName(_testCategory.Name), Times.Once);
    }

    [Fact]
    public void GetCategoriesByType_CallsRepositoryGetByType()
    {
        // Arrange
        var categories = new[] { _testCategory };
        _mockRepository.Setup(r => r.GetByType(_testCategory.Type)).Returns(categories);

        // Act
        var result = _facade.GetCategoriesByType(_testCategory.Type);

        // Assert
        Assert.Same(categories, result);
        _mockRepository.Verify(r => r.GetByType(_testCategory.Type), Times.Once);
    }

    [Fact]
    public void CreateCategory_CallsFactoryCreateAndRepositoryAdd()
    {
        // Arrange
        CategoryType type = CategoryType.Expense;
        string name = "New Category";
        Category newCategory = new Category(Guid.NewGuid(), type, name);
            
        // Act
        var result = _facade.CreateCategory(type, name);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(type, result.Type);
        Assert.Equal(name, result.Name);
        _mockRepository.Verify(r => r.Add(It.Is<Category>(c => 
            c.Type == type && c.Name == name)), Times.Once);
    }

    [Fact]
    public void UpdateCategoryName_CallsRepositoryGetByIdAndUpdate()
    {
        // Arrange
        string newName = "Updated Category";
            
        _mockRepository.Setup(r => r.GetById(_testCategory.Id)).Returns(_testCategory);

        // Act
        var result = _facade.UpdateCategoryName(_testCategory.Id, newName);

        // Assert
        Assert.Same(_testCategory, result);
        Assert.Equal(newName, _testCategory.Name);
        _mockRepository.Verify(r => r.GetById(_testCategory.Id), Times.Once);
        _mockRepository.Verify(r => r.Update(_testCategory), Times.Once);
    }

    [Fact]
    public void UpdateCategoryName_WhenCategoryNotFound_ThrowsArgumentException()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        string newName = "Updated Category";
            
        _mockRepository.Setup(r => r.GetById(id)).Returns((Category)null);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _facade.UpdateCategoryName(id, newName));
        Assert.Contains(id.ToString(), exception.Message);
    }

    [Fact]
    public void UpdateCategoryType_CallsRepositoryGetByIdAndUpdate()
    {
        // Arrange
        CategoryType newType = CategoryType.Expense;
            
        _mockRepository.Setup(r => r.GetById(_testCategory.Id)).Returns(_testCategory);

        // Act
        var result = _facade.UpdateCategoryType(_testCategory.Id, newType);

        // Assert
        Assert.Same(_testCategory, result);
        Assert.Equal(newType, _testCategory.Type);
        _mockRepository.Verify(r => r.GetById(_testCategory.Id), Times.Once);
        _mockRepository.Verify(r => r.Update(_testCategory), Times.Once);
    }

    [Fact]
    public void UpdateCategoryType_WhenCategoryNotFound_ThrowsArgumentException()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        CategoryType newType = CategoryType.Expense;
            
        _mockRepository.Setup(r => r.GetById(id)).Returns((Category)null);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _facade.UpdateCategoryType(id, newType));
        Assert.Contains(id.ToString(), exception.Message);
    }

    [Fact]
    public void DeleteCategory_CallsRepositoryDelete()
    {
        // Arrange
        _mockRepository.Setup(r => r.Delete(_testCategory.Id)).Returns(true);

        // Act
        var result = _facade.DeleteCategory(_testCategory.Id);

        // Assert
        Assert.True(result);
        _mockRepository.Verify(r => r.Delete(_testCategory.Id), Times.Once);
    }
}