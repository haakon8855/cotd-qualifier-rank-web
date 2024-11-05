namespace CotdQualifierRank.Domain.DomainPrimitives;

public class CompetitionMonth : DomainPrimitive
{
    private const int Min = 1;
    private const int Max = 12;

    public int Value { get; }

    public CompetitionMonth(int value)
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
}