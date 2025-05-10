namespace HSEBank.FinancialAccounting.Models;

public class BankAccount
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public decimal Balance { get; private set; }
    
    private BankAccount() { }

    internal BankAccount(Guid id, string name, decimal balance)
    {
        Id = id;
        Name = name;
        Balance = balance;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Account name cannot be empty", nameof(name));
        }

        Name = name;
    }

    public void UpdateBalance(decimal balance)
    {
        Balance = balance;
    }

    public void AddToBalance(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(amount));
        }

        Balance += amount;
    }

    public void SubtractFromBalance(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(amount));
        }

        Balance -= amount;
    }
}