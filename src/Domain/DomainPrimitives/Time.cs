namespace CotdQualifierRank.Domain.DomainPrimitives;

public class Time : DomainPrimitive
{
    private const int Min = 0;
    private const int Max = 3600000;
    public int Value { get; }

    public Time()
    {
        AssertValid(0);
        Value = 0;
    }
    
    public Time(int value)
    {
        AssertValid(value);
        Value = value;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return $"{Value}";
    }

    private static void AssertValid(int value)
    {
        if (!IsValid(value))
            throw new DomainPrimitiveArgumentException<int>(value);
    }

    public static bool IsValid(int value)
    {
        return value is >= Min and <= Max;
    }

    public static bool operator <(Time left, Time right)
    {
        return left.Value < right.Value;
    }

    public static bool operator >(Time left, Time right)
    {
        return left.Value > right.Value;
    }
}