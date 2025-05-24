using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;
using HSEBank.FinancialAccounting.Proxies;
using Moq;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Proxies;

public class CachedCategoryRepositoryTests
{
    private readonly Mock<ICategoryRepository> _mockInnerRepository;
    private readonly CachedCategoryRepository _cachedRepository;
    private readonly Category _testCategory;

    public CachedCategoryRepositoryTests()
    {
        _mockInnerRepository = new Mock<ICategoryRepository>();
        _cachedRepository = new CachedCategoryRepository(_mockInnerRepository.Object);
        _testCategory = new Category(Guid.NewGuid(), CategoryType.Income, "Test Category");
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CachedCategoryRepository(null));
    }

    [Fact]
    public void GetAll_FirstCall_CallsInnerRepository()
    {
        // Arrange
        var categories = new List<Category> { _testCategory };
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(categories);

        // Act
        var result = _cachedRepository.GetAll();

        // Assert
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.Equal(categories, result);
    }

    [Fact]
    public void GetAll_SecondCall_UsesCache()
    {
        // Arrange
        var categories = new List<Category> { _testCategory };
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(categories);

        // Act
        _cachedRepository.GetAll();
        var result = _cachedRepository.GetAll();

        // Assert
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.Equal(categories, result);
    }

    [Fact]
    public void GetById_FirstCall_InitializesCache()
    {
        // Arrange
        var categoryId = _testCategory.Id;
        var categories = new List<Category> { _testCategory };
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(categories);

        // Act
        var result = _cachedRepository.GetById(categoryId);

        // Assert
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.Equal(_testCategory, result);
    }

    [Fact]
    public void GetById_SecondCall_UsesCache()
    {
        // Arrange
        var categoryId = _testCategory.Id;
        var categories = new List<Category> { _testCategory };
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(categories);

        // Act
        _cachedRepository.GetById(categoryId);
        var result = _cachedRepository.GetById(categoryId);

        // Assert
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.Equal(_testCategory, result);
    }

    [Fact]
    public void GetById_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var categories = new List<Category> { _testCategory };
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(categories);

        // Act
        var result = _cachedRepository.GetById(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetByName_FirstCall_InitializesCache()
    {
        // Arrange
        var categoryName = "Test Category";
        var categories = new List<Category> { _testCategory };
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(categories);

        // Act
        var result = _cachedRepository.GetByName(categoryName);

        // Assert
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.Equal(_testCategory, result);
    }

    [Fact]
    public void GetByName_SecondCall_UsesCache()
    {
        // Arrange
        var categoryName = "Test Category";
        var categories = new List<Category> { _testCategory };
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(categories);

        // Act
        _cachedRepository.GetByName(categoryName);
        var result = _cachedRepository.GetByName(categoryName);

        // Assert
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.Equal(_testCategory, result);
    }

    [Fact]
    public void GetByType_FirstCall_InitializesCache()
    {
        // Arrange
        var categoryType = CategoryType.Income;
        var categories = new List<Category> { _testCategory };
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(categories);

        // Act
        var result = _cachedRepository.GetByType(categoryType);

        // Assert
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.Contains(_testCategory, result);
    }

    [Fact]
    public void GetByType_SecondCall_UsesCache()
    {
        // Arrange
        var categoryType = CategoryType.Income;
        var categories = new List<Category> { _testCategory };
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(categories);

        // Act
        _cachedRepository.GetByType(categoryType);
        var result = _cachedRepository.GetByType(categoryType);

        // Assert
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.Contains(_testCategory, result);
    }

    [Fact]
    public void Add_CallsInnerRepository_AndInvalidatesCache()
    {
        // Arrange
        var initialCategories = new List<Category>();
        var updatedCategories = new List<Category> { _testCategory };
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(initialCategories);
        _mockInnerRepository.Setup(r => r.Add(It.IsAny<Category>()));
            
        // Act
        _cachedRepository.GetAll();
        _cachedRepository.Add(_testCategory);
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(updatedCategories);
            
        // Assert
        _mockInnerRepository.Verify(r => r.Add(It.IsAny<Category>()), Times.Once);
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(updatedCategories);
        var allCategories = _cachedRepository.GetAll();
        Assert.Equal(updatedCategories, allCategories);
    }

    [Fact]
    public void Update_CallsInnerRepository_AndInvalidatesCache()
    {
        // Arrange
        var categories = new List<Category> { _testCategory };
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(categories);
        _mockInnerRepository.Setup(r => r.Update(It.IsAny<Category>()));
            
        // Act
        _cachedRepository.GetAll();
        _cachedRepository.Update(_testCategory);
            
        // Assert
        _mockInnerRepository.Verify(r => r.Update(It.IsAny<Category>()), Times.Once);
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(categories);
        var allCategories = _cachedRepository.GetAll();
        Assert.Equal(categories, allCategories);
    }

    [Fact]
    public void Delete_CallsInnerRepository_AndInvalidatesCache()
    {
        // Arrange
        var categoryId = _testCategory.Id;
        var initialCategories = new List<Category> { _testCategory };
        var updatedCategories = new List<Category>();
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(initialCategories);
        _mockInnerRepository.Setup(r => r.Delete(It.IsAny<Guid>())).Returns(true);
            
        // Act
        _cachedRepository.GetAll();
        var result = _cachedRepository.Delete(categoryId);
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(updatedCategories);
            
        // Assert
        _mockInnerRepository.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Once);
        _mockInnerRepository.Verify(r => r.GetAll(), Times.Once);
        Assert.True(result);
            
        _mockInnerRepository.Setup(r => r.GetAll()).Returns(updatedCategories);
        var allCategories = _cachedRepository.GetAll();
        Assert.Empty(allCategories);
    }
}