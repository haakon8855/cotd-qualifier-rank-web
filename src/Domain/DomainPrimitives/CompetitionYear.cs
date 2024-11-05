namespace CotdQualifierRank.Domain.DomainPrimitives;

public class CompetitionYear : DomainPrimitive
{
    private const int Min = 2020;
    private const int Max = 2077;

    public int Value { get; }

    public CompetitionYear(int value)
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