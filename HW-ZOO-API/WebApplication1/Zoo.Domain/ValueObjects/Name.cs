namespace Zoo.Domain.ValueObjects;

public class Name
{
    public string Value { get; }

    private Name(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > 30)
        {
            throw new ArgumentException("Name cannot be empty or longer than 30 characters", nameof(name));
        }

        Value = name;
    }
    
    public static Name FromString(string name)
    {
        return new Name(name);
    }

    public override string ToString()
    {
        return Value;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is Name name)
        {
            return Value == name.Value;
        }

        return false;
    }
    
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}