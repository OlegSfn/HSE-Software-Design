namespace S1.PedalCarAccountingInformationSystem;

public class Customer
{
    public required string Name { get; init; }

    public Car? Car { get; set; }

    public override string ToString()
    {
        return $"Имя: {Name}, Номер машины: {Car?.Number ?? -1}";
    }
}
