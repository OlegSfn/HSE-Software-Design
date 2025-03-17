using HSEBank.FinancialAccounting.Interfaces;
using HSEBank.FinancialAccounting.Interfaces.Repositories;
using HSEBank.FinancialAccounting.Models;

namespace HSEBank.FinancialAccounting.Repositories;

public class InMemoryBankAccountRepository : IBankAccountRepository
{
    private readonly Dictionary<Guid, BankAccount> _bankAccounts = new();
    
    public IEnumerable<BankAccount> GetAll() => _bankAccounts.Values;

    public BankAccount GetById(Guid id) 
        => _bankAccounts.TryGetValue(id, out var bankAccount) ? bankAccount : null;
    
    public BankAccount GetByName(string name)
        => _bankAccounts.Values.FirstOrDefault(ba => ba.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public void Add(BankAccount bankAccount)
    {
        if (bankAccount == null)
        {
            throw new ArgumentNullException(nameof(bankAccount));
        }

        if (_bankAccounts.ContainsKey(bankAccount.Id))
        {
            throw new ArgumentException($"Bank account with ID {bankAccount.Id} already exists");
        }

        _bankAccounts[bankAccount.Id] = bankAccount;
    }

    public void Update(BankAccount bankAccount)
    {
        if (bankAccount == null)
        {
            throw new ArgumentNullException(nameof(bankAccount));
        }

        if (!_bankAccounts.ContainsKey(bankAccount.Id))
        {
            throw new ArgumentException($"Bank account with ID {bankAccount.Id} does not exist");
        }

        _bankAccounts[bankAccount.Id] = bankAccount;
    }

    public bool Delete(Guid id) 
        => _bankAccounts.Remove(id);
}