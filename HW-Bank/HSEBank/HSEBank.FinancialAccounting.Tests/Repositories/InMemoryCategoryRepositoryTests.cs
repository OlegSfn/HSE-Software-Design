using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;
using HSEBank.FinancialAccounting.Repositories;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Repositories;

public class InMemoryCategoryRepositoryTests
{
    private readonly InMemoryCategoryRepository _repository;
    private readonly Category _testCategory;
    private readonly Guid _categoryId = Guid.NewGuid();

    public InMemoryCategoryRepositoryTests()
    {
        _repository = new InMemoryCategoryRepository();
        _testCategory = new Category(_categoryId, CategoryType.Income, "Test Category");
            
        _repository.Add(_testCategory);
    }

    [Fact]
    public void GetAll_ReturnsAllCategories()
    {
        // Arrange
        var additionalCategory = new Category(Guid.NewGuid(), CategoryType.Expense, "Expense Category");
        _repository.Add(additionalCategory);

        // Act
        var result = _repository.GetAll();

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(_testCategory, result);
        Assert.Contains(additionalCategory, result);
    }

    [Fact]
    public void GetById_WithExistingId_ReturnsCategory()
    {
        // Act
        var result = _repository.GetById(_categoryId);

        // Assert
        Assert.Equal(_testCategory, result);
    }

    [Fact]
    public void GetById_WithNonExistentId_ReturnsNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = _repository.GetById(nonExistentId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetByName_WithExistingName_ReturnsCategory()
    {
        // Act
        var result = _repository.GetByName("Test Category");

        // Assert
        Assert.Equal(_testCategory, result);
    }

    [Fact]
    public void GetByName_WithNonExistentName_ReturnsNull()
    {
        // Act
        var result = _repository.GetByName("Non-existent Category");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetByName_IsCaseInsensitive()
    {
        // Act
        var result = _repository.GetByName("test category");

        // Assert
        Assert.Equal(_testCategory, result);
    }

    [Fact]
    public void GetByType_ReturnsAllCategoriesOfSpecifiedType()
    {
        // Arrange
        var incomeCategory2 = new Category(Guid.NewGuid(), CategoryType.Income, "Income Category 2");
        var expenseCategory = new Category(Guid.NewGuid(), CategoryType.Expense, "Expense Category");
            
        _repository.Add(incomeCategory2);
        _repository.Add(expenseCategory);

        // Act
        var incomeCategories = _repository.GetByType(CategoryType.Income);
        var expenseCategories = _repository.GetByType(CategoryType.Expense);

        // Assert
        Assert.Equal(2, incomeCategories.Count());
        Assert.Single(expenseCategories);
            
        Assert.Contains(_testCategory, incomeCategories);
        Assert.Contains(incomeCategory2, incomeCategories);
        Assert.Contains(expenseCategory, expenseCategories);
    }

    [Fact]
    public void Add_WithValidCategory_AddsToRepository()
    {
        // Arrange
        var newCategory = new Category(Guid.NewGuid(), CategoryType.Expense, "New Category");

        // Act
        _repository.Add(newCategory);
        var retrievedCategory = _repository.GetById(newCategory.Id);

        // Assert
        Assert.NotNull(retrievedCategory);
        Assert.Equal(newCategory, retrievedCategory);
    }

    [Fact]
    public void Add_WithNullCategory_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _repository.Add(null));
    }

    [Fact]
    public void Add_WithDuplicateId_ThrowsArgumentException()
    {
        // Arrange
        var duplicateCategory = new Category(_categoryId, CategoryType.Expense, "Duplicate ID");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _repository.Add(duplicateCategory));
    }

    [Fact]
    public void Update_WithExistingCategory_UpdatesCategory()
    {
        // Arrange
        var updatedCategory = new Category(_categoryId, CategoryType.Expense, "Updated Category");

        // Act
        _repository.Update(updatedCategory);
        var retrievedCategory = _repository.GetById(_categoryId);

        // Assert
        Assert.Equal(updatedCategory, retrievedCategory);
        Assert.Equal("Updated Category", retrievedCategory.Name);
        Assert.Equal(CategoryType.Expense, retrievedCategory.Type);
    }

    [Fact]
    public void Update_WithNullCategory_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => _repository.Update(null));
    }

    [Fact]
    public void Update_WithNonExistentId_ThrowsArgumentException()
    {
        // Arrange
        var nonExistentCategory = new Category(Guid.NewGuid(), CategoryType.Income, "Non-existent Category");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _repository.Update(nonExistentCategory));
    }

    [Fact]
    public void Delete_WithExistingId_RemovesCategory()
    {
        // Act
        var result = _repository.Delete(_categoryId);
        var retrievedCategory = _repository.GetById(_categoryId);

        // Assert
        Assert.True(result);
        Assert.Null(retrievedCategory);
    }

    [Fact]
    public void Delete_WithNonExistentId_ReturnsFalse()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = _repository.Delete(nonExistentId);

        // Assert
        Assert.False(result);
    }
}