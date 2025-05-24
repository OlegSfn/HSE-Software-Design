using HSEBank.FinancialAccounting.Commands;
using HSEBank.FinancialAccounting.Facades;
using HSEBank.FinancialAccounting.Factories;
using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;
using Moq;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Commands;

public class CreateCategoryCommandTests
{
    private readonly Mock<ICategoryRepository> _mockRepository;
    private readonly CategoryFacade _facade;
    private readonly CreateCategoryCommand _command;
    private readonly CategoryType _type;
    private readonly string _name;

    public CreateCategoryCommandTests()
    {
        _mockRepository = new Mock<ICategoryRepository>();
        var factory = new CategoryFactory();
        _facade = new CategoryFacade(_mockRepository.Object, factory);
        _type = CategoryType.Expense;
        _name = "Test Category";
        _command = new CreateCategoryCommand(_facade, _type, _name);
    }

    [Fact]
    public void Constructor_WithNullFacade_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CreateCategoryCommand(null, _type, _name));
    }

    [Fact]
    public void Constructor_WithNullName_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CreateCategoryCommand(_facade, _type, null));
    }

    [Fact]
    public void Execute_CallsRepositoryAdd()
    {
        // Act
        _command.Execute();

        // Assert
        _mockRepository.Verify(r => r.Add(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public void Undo_WhenCategoryCreated_CallsRepositoryDelete()
    {
        // Arrange
        _command.Execute();
        var createdCategoryId = _command.CreatedCategory.Id;

        // Act
        _command.Undo();

        // Assert
        _mockRepository.Verify(r => r.Delete(createdCategoryId), Times.Once);
    }

    [Fact]
    public void Undo_WhenCategoryNotCreated_DoesNothing()
    {
        // Act
        _command.Undo();

        // Assert
        _mockRepository.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public void CreatedCategory_BeforeExecute_ReturnsNull()
    {
        // Act & Assert
        Assert.Null(_command.CreatedCategory);
    }

    [Fact]
    public void CreatedCategory_AfterExecute_ReturnsCategory()
    {
        // Act
        _command.Execute();

        // Assert
        Assert.NotNull(_command.CreatedCategory);
        Assert.Equal(_type, _command.CreatedCategory.Type);
        Assert.Equal(_name, _command.CreatedCategory.Name);
    }

    [Fact]
    public void CreatedCategory_AfterUndo_ReturnsNull()
    {
        // Arrange
        _command.Execute();
        _mockRepository.Setup(r => r.Delete(It.IsAny<Guid>())).Returns(true);

        // Act
        _command.Undo();

        // Assert
        Assert.Null(_command.CreatedCategory);
    }
}