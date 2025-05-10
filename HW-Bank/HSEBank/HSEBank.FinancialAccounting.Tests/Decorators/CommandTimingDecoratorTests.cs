using HSEBank.FinancialAccounting.Commands;
using HSEBank.FinancialAccounting.Decorators;
using Moq;
using Xunit;

namespace HSEBank.FinancialAccounting.Tests.Decorators;

public class CommandTimingDecoratorTests
{
    private readonly Mock<ICommand> _mockCommand;
    private readonly Mock<Action<string, TimeSpan>> _mockLogAction;
    private readonly CommandTimingDecorator _decorator;

    public CommandTimingDecoratorTests()
    {
        _mockCommand = new Mock<ICommand>();
        _mockLogAction = new Mock<Action<string, TimeSpan>>();
        _decorator = new CommandTimingDecorator(_mockCommand.Object, _mockLogAction.Object);
    }

    [Fact]
    public void Constructor_WithNullCommand_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CommandTimingDecorator(null, _mockLogAction.Object));
    }

    [Fact]
    public void Constructor_WithNullLogAction_ThrowsArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new CommandTimingDecorator(_mockCommand.Object, null));
    }

    [Fact]
    public void Execute_CallsCommandExecute_AndLogsExecutionTime()
    {
        // Arrange
        _mockCommand.Setup(c => c.Execute()).Callback(() => Thread.Sleep(10));

        // Act
        _decorator.Execute();

        // Assert
        _mockCommand.Verify(c => c.Execute(), Times.Once);
        _mockLogAction.Verify(l => l.Invoke(It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public void Execute_WhenCommandThrowsException_PropagatesException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        _mockCommand.Setup(c => c.Execute()).Throws(expectedException);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => _decorator.Execute());
        Assert.Same(expectedException, exception);
        _mockCommand.Verify(c => c.Execute(), Times.Once);
        _mockLogAction.Verify(l => l.Invoke(It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Never);
    }

    [Fact]
    public void Undo_CallsCommandUndo_AndLogsExecutionTime()
    {
        // Arrange
        _mockCommand.Setup(c => c.Undo()).Callback(() => System.Threading.Thread.Sleep(10));

        // Act
        _decorator.Undo();

        // Assert
        _mockCommand.Verify(c => c.Undo(), Times.Once);
        _mockLogAction.Verify(l => l.Invoke(It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public void Undo_WhenCommandThrowsException_PropagatesException()
    {
        // Arrange
        var expectedException = new InvalidOperationException("Test exception");
        _mockCommand.Setup(c => c.Undo()).Throws(expectedException);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => _decorator.Undo());
        Assert.Same(expectedException, exception);
        _mockCommand.Verify(c => c.Undo(), Times.Once);
        _mockLogAction.Verify(l => l.Invoke(It.IsAny<string>(), It.IsAny<TimeSpan>()), Times.Never);
    }

    [Fact]
    public void Execute_LogsCommandTypeName()
    {
        // Arrange
        string capturedCommandName = null;
        _mockLogAction.Setup(l => l.Invoke(It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .Callback<string, TimeSpan>((name, time) => capturedCommandName = name);

        // Act
        _decorator.Execute();

        // Assert
        Assert.NotNull(capturedCommandName);
        Assert.Contains("Executed", capturedCommandName);
        Assert.Contains(_mockCommand.Object.GetType().Name, capturedCommandName);
    }

    [Fact]
    public void Undo_LogsCommandTypeName()
    {
        // Arrange
        string capturedCommandName = null;
        _mockLogAction.Setup(l => l.Invoke(It.IsAny<string>(), It.IsAny<TimeSpan>()))
            .Callback<string, TimeSpan>((name, time) => capturedCommandName = name);

        // Act
        _decorator.Undo();

        // Assert
        Assert.NotNull(capturedCommandName);
        Assert.Contains("Undone", capturedCommandName);
        Assert.Contains(_mockCommand.Object.GetType().Name, capturedCommandName);
    }
}