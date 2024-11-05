namespace CotdQualifierRank.Domain.DomainPrimitives;

public class NadeoCompetitionId : DomainPrimitive
{
    public int Value { get; }

    public NadeoCompetitionId(int value)
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
        return value >= 0;
    }
}