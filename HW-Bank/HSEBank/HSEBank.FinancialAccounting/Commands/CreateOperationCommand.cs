using HSEBank.FinancialAccounting.Interfaces.Facades;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.Commands;

public class CreateOperationCommand : ICommand
{
    private readonly IOperationFacade _operationFacade;
    private readonly OperationType _type;
    private readonly Guid _bankAccountId;
    private readonly decimal _amount;
    private readonly DateTime _date;
    private readonly Guid _categoryId;
    private readonly string _description;
    private Operation? _createdOperation;

    public Operation? CreatedOperation => _createdOperation;
        
    public CreateOperationCommand(
        IOperationFacade operationFacade,
        OperationType type,
        Guid bankAccountId,
        decimal amount,
        DateTime date,
        Guid categoryId,
        string description = null)
    {
        _operationFacade = operationFacade ?? throw new ArgumentNullException(nameof(operationFacade));
        _type = type;
        _bankAccountId = bankAccountId;
        _amount = amount;
        _date = date;
        _categoryId = categoryId;
        _description = description;
    }

    public void Execute()
    {
        _createdOperation = _operationFacade.CreateOperation(_type, _bankAccountId, _amount, _date, _categoryId, _description);
    }

    public void Undo()
    {
        if (_createdOperation == null)
        {
            return;
        }
            
        _operationFacade.DeleteOperation(_createdOperation.Id);
        _createdOperation = null;
    }
}