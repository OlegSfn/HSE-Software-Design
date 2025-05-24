using HSEBank.FinancialAccounting.Interfaces.Facades;
using HSEBank.FinancialAccounting.Models;

namespace HSEBank.FinancialAccounting.Commands;

public class CreateBankAccountCommand : ICommand
{
    private readonly IBankAccountFacade _bankAccountFacade;
    private readonly string _name;
    private readonly decimal _initialBalance;
    private BankAccount? _createdBankAccount;
        
    public BankAccount? CreatedBankAccount => _createdBankAccount;
        
    public CreateBankAccountCommand(IBankAccountFacade bankAccountFacade, string name, decimal initialBalance)
    {
        _bankAccountFacade = bankAccountFacade ?? throw new ArgumentNullException(nameof(bankAccountFacade));
        _name = name ?? throw new ArgumentNullException(nameof(name));
        _initialBalance = initialBalance;
    }

    public void Execute()
    {
        _createdBankAccount = _bankAccountFacade.CreateBankAccount(_name, _initialBalance);
    }

    public void Undo()
    {
        if (_createdBankAccount == null)
        {
            return;
        }
            
        _bankAccountFacade.DeleteBankAccount(_createdBankAccount.Id);
        _createdBankAccount = null;
    }
}