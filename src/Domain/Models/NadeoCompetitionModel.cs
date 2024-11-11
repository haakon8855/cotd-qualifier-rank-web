using System.Text.RegularExpressions;

namespace CotdQualifierRank.Domain.Models;

public class NadeoCompetitionModel
{
    public NadeoCompetitionModel(
        int id,
        string? liveId,
        string? name,
        string? description,
        int nbPlayers)
    {
        Id = id;
        LiveId = liveId ?? string.Empty;
        Name = name ?? string.Empty;
        Description = description ?? string.Empty;
        NbPlayers = nbPlayers;
    }

    public int Id { get; set; }
    public string LiveId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int NbPlayers { get; set; }

    public static DateTime ParseDate(string input)
    {
        var nameFormat = @"20[0-9][0-9]-[0-9][0-9]-[0-9][0-9]";
        var match = Regex.Match(input, nameFormat);

        if (match.Success && DateTime.TryParse(match.Value, out DateTime date))
            return date;

        return DateTime.Parse("2020-07-01");
    }
}