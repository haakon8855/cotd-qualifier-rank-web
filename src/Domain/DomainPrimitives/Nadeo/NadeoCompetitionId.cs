namespace CotdQualifierRank.Domain.DomainPrimitives.Nadeo;

/*
 * The CompetitionId is the Id Nadeo uses as identifier for different competition.
 * In the context of COTD, a single COTD session is a single competition. While
 * COTM or COTN is in turn their own competitions with unique Ids.
 */
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