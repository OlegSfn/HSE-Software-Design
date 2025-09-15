using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.Models;

public class Operation
{
    public Guid Id { get; private set; }
    public OperationType Type { get; private set; }
    public Guid BankAccountId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }
    public string Description { get; private set; }
    public Guid CategoryId { get; private set; }

    private Operation() { }

    internal Operation(Guid id, OperationType type, Guid bankAccountId, decimal amount, DateTime date, Guid categoryId, string description = null)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(amount));
        }

        Id = id;
        Type = type;
        BankAccountId = bankAccountId;
        Amount = amount;
        Date = date;
        CategoryId = categoryId;
        Description = description;
    }

    public void UpdateType(OperationType type)
    {
        Type = type;
    }
    
    public void UpdateAmount(decimal amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(amount));
        }

        Amount = amount;
    }
    
    public void UpdateDate(DateTime date)
    {
        Date = date;
    }

    public void UpdateDescription(string description)
    {
        Description = description;
    }

    public void UpdateCategory(Guid categoryId)
    {
        CategoryId = categoryId;
    }
}