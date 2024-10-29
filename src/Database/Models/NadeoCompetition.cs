using System.Text.RegularExpressions;

namespace CotdQualifierRank.Database.Models;

public class NadeoCompetition
{
    public int Id { get; set; }
    public string? liveId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
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