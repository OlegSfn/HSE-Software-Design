namespace Zoo.Domain.ValueObjects;

public class NonNegativeInt
{
    public int Value { get; }
    
    private NonNegativeInt(int value)
    {
        if (value < 0)
        {
            throw new ArgumentException("Value cannot be negative", nameof(value));
        }
        
        Value = value;
    }
    
    public static NonNegativeInt FromInt(int value)
    {
        return new NonNegativeInt(value);
    }
    
    public override string ToString()
    {
        return Value.ToString();
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is NonNegativeInt nonNegativeInt)
        {
            return Value == nonNegativeInt.Value;
        }

        return false;
    }
    
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public int ToInt()
    {
        return Value;
    }
    
    public static bool operator ==(NonNegativeInt? left, NonNegativeInt? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(NonNegativeInt? left, NonNegativeInt? right)
    {
        return !(left == right);
    }

    public static bool operator <(NonNegativeInt? left, NonNegativeInt? right)
    {
        if (left is null) return right is not null;
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(NonNegativeInt? left, NonNegativeInt? right)
    {
        if (left is null) return true;
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(NonNegativeInt? left, NonNegativeInt? right)
    {
        return !(left <= right);
    }

    public static bool operator >=(NonNegativeInt? left, NonNegativeInt? right)
    {
        return !(left < right);
    }

    public int CompareTo(NonNegativeInt? other)
    {
        if (other is null) return 1;
        return Value.CompareTo(other.Value);
    }

    public int CompareTo(object? obj)
    {
        if (obj is null) return 1;
        if (obj is not NonNegativeInt other) 
            throw new ArgumentException("Object is not a NonNegativeInt");
        
        return CompareTo(other);
    }


}