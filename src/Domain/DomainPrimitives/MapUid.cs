using System.Text.RegularExpressions;

namespace CotdQualifierRank.Domain.DomainPrimitives;

public class MapUid : DomainPrimitive
{
    private const string MapUidPattern = @"^[A-Za-z0-9_]{26,27}$";
    public string Value { get; }

    public MapUid(string value)
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
        return Value;
    }

    private static void AssertValid(string value)
    {
        if (!IsValid(value))
            throw new DomainPrimitiveArgumentException<string>(value);
    }

    public static bool IsValid(string value)
    {
        return Regex.IsMatch(value, MapUidPattern);
    }
}