using System.Text.RegularExpressions;
using CotdQualifierRank.Domain.DomainPrimitives.Nadeo;

namespace CotdQualifierRank.Domain.Models;

public class NadeoCompetitionModel(
    NadeoCompetitionId id,
    string? liveId,
    string? name,
    string? description,
    int nbPlayers)
{
    public NadeoCompetitionId Id { get; } = id;
    public string LiveId { get; } = liveId ?? string.Empty;
    public string Name { get; } = name ?? string.Empty;
    public string Description { get; } = description ?? string.Empty;
    public int NbPlayers { get; } = nbPlayers;

    public static DateTime ParseDate(string input)
    {
        var nameFormat = @"20[0-9][0-9]-[0-9][0-9]-[0-9][0-9]";
        var match = Regex.Match(input, nameFormat);

        if (match.Success && DateTime.TryParse(match.Value, out DateTime date))
            return date;

        return DateTime.Parse("2020-07-01");
    }
}