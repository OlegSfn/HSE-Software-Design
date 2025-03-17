using HSEBank.FinancialAccounting.Interfaces.Facades;
using HSEBank.FinancialAccounting.Interfaces.Factories;
using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Models;

namespace HSEBank.FinancialAccounting.Facades;

public class BankAccountFacade : IBankAccountFacade
{
    private readonly IBankAccountRepository _bankAccountRepository;
    private readonly IBankAccountFactory _bankAccountFactory;

    public BankAccountFacade(IBankAccountRepository bankAccountRepository, IBankAccountFactory bankAccountFactory)
    {
        _bankAccountRepository = bankAccountRepository ?? throw new ArgumentNullException(nameof(bankAccountRepository));
        _bankAccountFactory = bankAccountFactory ?? throw new ArgumentNullException(nameof(bankAccountFactory));
    }

    public IEnumerable<BankAccount> GetAllBankAccounts()
    {
        return _bankAccountRepository.GetAll();
    }

    public BankAccount GetBankAccountById(Guid id)
    {
        return _bankAccountRepository.GetById(id);
    }

    public BankAccount GetBankAccountByName(string name)
    {
        return _bankAccountRepository.GetByName(name);
    }

    public BankAccount CreateBankAccount(string name, decimal initialBalance)
    {
        var bankAccount = _bankAccountFactory.Create(name, initialBalance);
        _bankAccountRepository.Add(bankAccount);
        
        return bankAccount;
    }

    public BankAccount UpdateBankAccountName(Guid id, string name)
    {
        var bankAccount = _bankAccountRepository.GetById(id);
        if (bankAccount == null)
        {
            throw new ArgumentException($"Bank account with ID {id} not found");
        }

        bankAccount.UpdateName(name);
        _bankAccountRepository.Update(bankAccount);
        
        return bankAccount;
    }

    public BankAccount UpdateBankAccountBalance(Guid id, decimal balance)
    {
        var bankAccount = _bankAccountRepository.GetById(id);
        if (bankAccount == null)
        {
            throw new ArgumentException($"Bank account with ID {id} not found");
        }

        bankAccount.UpdateBalance(balance);
        _bankAccountRepository.Update(bankAccount);
        
        return bankAccount;
    }
        
    public bool DeleteBankAccount(Guid id) => _bankAccountRepository.Delete(id);
}