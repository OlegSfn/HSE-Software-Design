using System.Diagnostics;
using HSEBank.FinancialAccounting.Commands;

namespace HSEBank.FinancialAccounting.Decorators;

public class CommandTimingDecorator : ICommand
{
    private readonly ICommand _command;
    private readonly Action<string, TimeSpan> _logAction;

    public CommandTimingDecorator(ICommand command, Action<string, TimeSpan> logAction)
    {
        _command = command ?? throw new ArgumentNullException(nameof(command));
        _logAction = logAction ?? throw new ArgumentNullException(nameof(logAction));
    }

    public void Execute()
    {
        var stopwatch = Stopwatch.StartNew();
        _command.Execute();
        stopwatch.Stop();
        _logAction($"Executed {_command.GetType().Name}", stopwatch.Elapsed);
    }

    public void Undo()
    {
        var stopwatch = Stopwatch.StartNew();
        _command.Undo();
        stopwatch.Stop();
        _logAction($"Undone {_command.GetType().Name}", stopwatch.Elapsed);
    }
}