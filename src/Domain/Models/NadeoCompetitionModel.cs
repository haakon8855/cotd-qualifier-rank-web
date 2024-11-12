using System.Text.RegularExpressions;
using CotdQualifierRank.Domain.DomainPrimitives.Nadeo;

namespace CotdQualifierRank.Domain.Models;

public class NadeoCompetitionModel
{
    public NadeoCompetitionModel(
        NadeoCompetitionId id,
        string? liveId,
        string? name,
        string? description,
        int nbPlayers)
    {
        Id = id;
        LiveId = liveId ?? "";
        Name = name ?? "";
        Description = description ?? "";
        NbPlayers = nbPlayers;
        Date = ParseDate(name ?? "");
    }

    public NadeoCompetitionModel(
        NadeoCompetitionId id,
        string? liveId,
        string? name,
        string? description,
        int nbPlayers,
        DateTime date)
    {
        Id = id;
        LiveId = liveId ?? "";
        Name = name ?? "";
        Description = description ?? "";
        NbPlayers = nbPlayers;
        Date = date;
    }

    public NadeoCompetitionId Id { get; }
    public string LiveId { get; }
    public string Name { get; }
    public string Description { get; }
    public int NbPlayers { get; }
    public DateTime Date { get; }

    public static DateTime ParseDate(string input)
    {
        var nameFormat = @"20[0-9][0-9]-[0-9][0-9]-[0-9][0-9]";
        var match = Regex.Match(input, nameFormat);

        if (match.Success && DateTime.TryParse(match.Value, out DateTime date))
            return date;

        return DateTime.Parse("2020-07-01");
    }
}