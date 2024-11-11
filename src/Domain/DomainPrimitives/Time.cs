namespace CotdQualifierRank.Domain.DomainPrimitives;

public class Time : DomainPrimitive, IComparable<Time>
{
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

    public int CompareTo(Time? other)
    {
        return other is null ? 1 : Value.CompareTo(other.Value);
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
        return value is >= -Max and <= Max;
    }

    public static bool operator <(Time left, Time right)
    {
        return left.Value < right.Value;
    }

    public static bool operator >(Time left, Time right)
    {
        return left.Value > right.Value;
    }

    public static Time operator -(Time left, Time right)
    {
        return new Time(left.Value - right.Value);
    }
    
    public string FormattedTime()
    {
        // Return string with time in format "ss.ttt" or "mm:ss.ttt" (if necessary):
        if (Value < 60000)
            return $"{Value / 1000}.{Value % 1000:000}";
        return $"{Value / 60000}:{Value / 1000 % 60:00}.{Value % 1000:000}";
    }

    public string FormattedDiffTime()
    {
        // Return string with time in format "+ss.ttt" or "+mm:ss.ttt" OR "-ss.ttt" or "-mm:ss:ttt" (if negative):
        if (Value < 0)
        {
            if (Value > -60000)
                return $"-{Value / -1000}.{Math.Abs(Value % -1000):000}";
            return $"-{Value / -60000}:{Math.Abs(Value / -1000 % 60):00}.{Math.Abs(Value % -1000):000}";
        }

        return "+" + FormattedTime();
    }
}