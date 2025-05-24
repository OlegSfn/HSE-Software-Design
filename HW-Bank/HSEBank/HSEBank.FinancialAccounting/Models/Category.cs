using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.Models;

public class Category
{
    public Guid Id { get; private set; }
    public CategoryType Type { get; private set; }
    public string Name { get; private set; }

    private Category() { }

    internal Category(Guid id, CategoryType type, string name)
    {
        Id = id;
        Type = type;
        Name = name;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name cannot be empty", nameof(name));
        }

        Name = name;
    }

    public void UpdateType(CategoryType type)
    {
        Type = type;
    }
}