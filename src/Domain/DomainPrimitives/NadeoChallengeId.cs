namespace CotdQualifierRank.Domain.DomainPrimitives;

/*
 * The ChallengeId is the Id Nadeo uses as ids for individual rounds in competitions.
 * In the context of COTD, the qualification phase its own challenge.
 */
public class NadeoChallengeId : DomainPrimitive
{
    public int Value { get; }

    public NadeoChallengeId(int value)
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