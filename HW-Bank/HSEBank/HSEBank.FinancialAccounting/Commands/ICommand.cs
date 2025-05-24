namespace HSEBank.FinancialAccounting.Commands;

public interface ICommand
{
    void Execute();
    void Undo();
}